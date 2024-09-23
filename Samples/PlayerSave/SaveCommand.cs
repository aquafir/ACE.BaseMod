using ACE.Database;
using ACE.Database.Entity;
using ACE.Database.Models.Auth;
using PlayerSave.Helpers;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using WeenieClassName = ACE.Entity.Enum.WeenieClassName;

namespace PlayerSave;

internal class SaveCommand
{
    private static readonly Regex _loadRegex = new Regex(@"(?<save>[^,]+),(\s*\+?(?<account>[\w\d]+)\s*,)?\s*(?<name>\w+)\s*", RegexOptions.Compiled);
    private static readonly Regex _saveRegex = new Regex(@"(?<name>(\w*\s?)+\w)(\s*,\s*\+?(?<save>[\w\d]+)\s*)?", RegexOptions.Compiled);

    [CommandHandler("load", AccessLevel.Developer, CommandHandlerFlag.None, -1, "", "/load <part of save name>, [account name|ID], <new character name>")]
    public static void HandleLoad(Session session, params string[] parameters)
    {
        var match = _loadRegex.Match(string.Join(' ', parameters));

        if (!match.Success)
        {
            ModManager.Log("/load <part of save name>, [account name|ID,] <new character name>");
            return;
        }

        var saveParam = match.Groups["save"].Value;
        var saves = GetSaves();
        var savePath = saves.Where(s => Path.GetFileNameWithoutExtension(s).Contains(saveParam, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        if (string.IsNullOrEmpty(savePath))
        {
            ModManager.Log($"Failed to find any \"{Settings.Extension}\" save of {saves.Count} found matching \"{saveParam}\"");
            return;
        }

        PlayerSave save;
        try
        {
            var data = File.ReadAllBytes(savePath);
            var text = data.GZipToString();
            save = JsonSerializer.Deserialize<PlayerSave>(text);
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to deserialize \"{savePath}\"");
            return;
        }

        var nameParam = match.Groups["name"].Value;
        var accountParam = match.Groups["account"].Value;
        uint accountId = 0;
        Account account = null;

        if (!match.Groups["account"].Success)
            account = DatabaseManager.Authentication.GetAccountById(save.Character.AccountId);
        else if (uint.TryParse(accountParam, out accountId))
            account = DatabaseManager.Authentication.GetAccountById(accountId);
        else
            account = DatabaseManager.Authentication.GetAccountByName(accountParam);

        if (account is null)
        {
            ModManager.Log($"Failed to find account by name or ID: \"{accountParam}\"");
            return;
        }

        //Todo: decide about replacing existing characters
        if (PlayerManager.IsAccountAtMaxCharacterSlots(account.AccountName))
        {
            ModManager.Log($"Max number of characters-per-account ({(int)PropertyManager.GetLong("max_chars_per_account").Item}) already reached for account: \"{account.AccountName}\"");
            return;
        }

        if (PlayerManager.FindByName(nameParam) is not null)
        {
            ModManager.Log($"A character named \"{nameParam}\" exists already.");
            return;
        }

        DoLoad(save, nameParam, account, PatchClass.Settings.Options);
    }

    public static void DoLoad(PlayerSave save, string newCharName, Account account, LoadOptions options)
    {
        //Copy Character template to a new account ID, name, and GUID 
        //Character existingCharacter = save.Character;
        var newCharacter = save.Character.CopyCharacterAs(account.AccountId, newCharName, options);
        var newPlayerBiota = save.PlayerBiota.CopyBiotaAs(newCharacter, options);

        var idSwaps = new ConcurrentDictionary<uint, uint>();
        idSwaps[save.Character.Id] = newCharacter.Id;

        var newInventoryItems = options.IncludeInventory ? save.Inventory : new();
        newInventoryItems.AssignNewGuids(ref idSwaps, true);
        newInventoryItems.ChangeOwner(newCharacter, idSwaps, true);

        var newWieldedItems = options.IncludeWielded ? save.Wielded : new();
        //Old method skipped W_DEED_CLASS weenies, lazy approach.  Todo: add more support via SaveOptions
        newWieldedItems = newWieldedItems.Where(w => w.WeenieClassId != (uint)WeenieClassName.W_DEED_CLASS).ToList();
        newWieldedItems.AssignNewGuids(ref idSwaps);
        newWieldedItems.ChangeOwner(newCharacter, idSwaps);

        Player newPlayer = newPlayerBiota.CreatePlayer(newInventoryItems, newWieldedItems, newCharacter, null);
        newPlayer.SwapShortcuts(idSwaps);
        newPlayer.SwapEnchantmentRegistry(idSwaps);

        var possessions = newPlayer.GetAllPossessions();
        var possessedBiotas = new Collection<(ACE.Entity.Models.Biota biota, ReaderWriterLockSlim rwLock)>();
        foreach (var possession in possessions)
            possessedBiotas.Add((possession.Biota, possession.BiotaDatabaseLock));

        //We must await here--
        DatabaseManager.Shard.AddCharacterInParallel(newPlayer.Biota, newPlayer.BiotaDatabaseLock, possessedBiotas, newPlayer.Character, newPlayer.CharacterDatabaseLock, saveSuccess =>
        {
            if (!saveSuccess)
            {
                ModManager.Log($"Failed to load {save.Character.Name} as {newCharName} on account {account.AccountId}.");
                return;
            }

            PlayerManager.AddOfflinePlayer(newPlayer);

            var foundActiveSession = ACE.Server.Network.Managers.NetworkManager.Find(account.AccountId);
            if (foundActiveSession != null)
                foundActiveSession.Characters.Add(newPlayer.Character);

            ModManager.Log($"Loaded {save.Character.Name} as {newCharName} on account {account.AccountId}.");
        });
    }

    [CommandHandler("save", AccessLevel.Developer, CommandHandlerFlag.None, -1, "", "/save <player name>[, save name]")]
    public static void HandleSave(Session session, params string[] parameters)
    {
        var match = _saveRegex.Match(string.Join(' ', parameters));

        //        Did a quick Biota->Weenie conversion-> export thing yesterday after Smiley mentioned it the other day.  
        //Any worth in making an export command for that?

        //Messed around with per - player per - spell variations:
        //https://user-images.githubusercontent.com/83029060/202056553-bad9fd90-f169-40c6-802b-87f991f1eb67.mp4

        if (!match.Success)
        {
            ModManager.Log($"Usage: /save <player name>[, save name]");
            return;
        }

        //Get valid name
        var name = GetFormattedName(match.Groups["name"].Value);

        //Check taboo
        if (!IsLegalName(name))
            return;

        var player = PlayerManager.FindByName(name);

        if (player is null)
        {
            ModManager.Log($"Failed to save.  Player not found: {name}");
            return;
        }

        //Save if currently online before creating a save file
        var online = PlayerManager.GetOnlinePlayer(player.Name);
        if (online is not null)
        {
            online?.SavePlayerToDatabase();
            online?.SendMessage($"Database updated before saving to file.");
        }

        DatabaseManager.Shard.GetCharacter(player.Guid.Full, character =>
        {
            if (character is null) return;

            DatabaseManager.Shard.GetPossessedBiotasInParallel(character.Id, possessions =>
            {
                var playerBiota = DatabaseManager.Shard.BaseDatabase.GetBiota(character.Id);

                var saveName = match.Groups["save"].Value;
                if (string.IsNullOrEmpty(saveName))
                    DoSave(character, playerBiota, possessions);
                else
                    DoSave(character, playerBiota, possessions, saveName);
            });
        });
    }

    /// <summary>
    /// Create a GZipped save of a player from their Character, Biota, and possessions
    /// </summary>
    private static void DoSave(Character character, ACE.Database.Models.Shard.Biota playerBiota, PossessedBiotas possessions, string savePath = null)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,                                           //Make it readble if extracted
            ReferenceHandler = ReferenceHandler.IgnoreCycles,               //Required to prevent parent references
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull    //Skip nulls
        };

        //Todo: Do a proper BinaryWriter/Reader        
        var save = new PlayerSave
        {
            Character = character,
            PlayerBiota = playerBiota,
            Wielded = possessions.WieldedItems,
            Inventory = possessions.Inventory,
        };

        if (string.IsNullOrEmpty(savePath))
            savePath = Path.Combine(Mod.Instance.ModPath, "Saves", $"{character.Name} - {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}");
        else
            savePath = Path.Combine(Mod.Instance.ModPath, "Saves", savePath);

        var sb = new StringBuilder("\r\n");
        var watch = Stopwatch.StartNew();
        var jsonPath = $"{savePath}.json";
        var saveJson = JsonSerializer.Serialize(save, jsonOptions);
        watch.Stop();
        sb.AppendLine($"\r\nJSON size (kb): {Encoding.Unicode.GetByteCount(saveJson) / 1024,-20}Time (ms):{watch.ElapsedMilliseconds}");

        watch = Stopwatch.StartNew();
        var gzipPath = $"{savePath}{Settings.Extension}";
        var saveGZip = saveJson.ToGZip();
        watch.Stop();
        sb.AppendLine($"GZIP size (kb): {saveGZip.Length / 1024,-20}Time (ms):{watch.ElapsedMilliseconds}");

        watch = Stopwatch.StartNew();
        var binaryPath = $"{savePath}.bin";
        var saveBinary = PlayerHelpers.CreateBinarySave(character, playerBiota, possessions);
        watch.Stop();
        sb.AppendLine($"BIN  size (kb): {saveBinary.Length / 1024,-20}Time (ms):{watch.ElapsedMilliseconds}");

        watch = Stopwatch.StartNew();
        var gzipBinPath = $"{savePath}.gzbin";
        var gzipSaveBinary = Compression.CompressGzip(saveBinary);
        watch.Stop();
        sb.AppendLine($"ZBIN size (kb): {gzipSaveBinary.Length / 1024,-20}Time (ms):{watch.ElapsedMilliseconds}");

        ModManager.Log(sb.ToString());

        try
        {
            File.WriteAllText(jsonPath, saveJson);
            File.WriteAllBytes(gzipPath, saveGZip);
            File.WriteAllBytes(binaryPath, saveBinary);
            File.WriteAllBytes(gzipBinPath, gzipSaveBinary);
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to save {character.Name} to:\n{savePath}", ModManager.LogLevel.Error);
        }
    }

    private static bool IsLegalName(string name)
    {
        if (PropertyManager.GetBool("taboo_table").Item && DatManager.PortalDat.TabooTable.ContainsBadWord(name.ToLowerInvariant()))
        {
            ModManager.Log($"Rejecting name using something from the taboo table.", ModManager.LogLevel.Warn);
            return false;
        }

        if (PropertyManager.GetBool("creature_name_check").Item && DatabaseManager.World.IsCreatureNameInWorldDatabase(name))
        {
            ModManager.Log($"Rejecting name that matches a creature.", ModManager.LogLevel.Warn);
            return false;
        }

        if (name.Length > 32)
        {
            ModManager.Log($"Rejecting name that exceeds 32 characters.", ModManager.LogLevel.Warn);
            return false;
        }

        return true;
    }
    private static string GetFormattedName(string originalName) => originalName.Remove(1).ToUpper() + originalName.Substring(1);
    private static List<string> GetSaves() => Directory.GetFiles(PatchClass.Settings.SaveDirectory).Where(x => Path.GetExtension(x).Contains(Settings.Extension, StringComparison.OrdinalIgnoreCase)).ToList();
}
