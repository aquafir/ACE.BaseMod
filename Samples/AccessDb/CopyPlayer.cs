using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using ACE.Database;
using ACE.Entity.Enum.Properties;
using ACE.Server.Managers;
using ACE.Database.Entity;
using ACE.Database.Models.Shard;
using Biota = ACE.Entity.Models.Biota;


namespace AccessDb;

internal class SavePlayer
{
    public static string _saveDirectory = Path.Combine(Mod.ModPath, "Saves");

    /// <summary>
    /// Local copy of AdminCommands.HandleCopychar
    /// </summary>
    /// 

    // bornagain deletedCharID[, newCharName[, accountName]]
    //[CommandHandler("bornagain", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 1,
    //    "Restores a deleted character to an account.",
    //    "deletedCharID(, newCharName)(, accountName)\n" +
    //    "Given the ID of a deleted character, this command restores that character to its owner.  (You can find the ID of a deleted character using the @finger command.)\n" +
    //    "If the deleted character's name has since been taken by a new character, you can specify a new name for the restored character as the second parameter.  (You can find if the name has been taken by also using the @finger command.)  Use a comma to separate the arguments.\n" +
    //    "If needed, you can specify an account name as a third parameter if the character should be restored to an account other than its original owner.  Again, use a comma between the arguments.")]
    //public static void HandleBornAgain(Session session, params string[] parameters)
    //{
    //    // usage: @bornagain deletedCharID[, newCharName[, accountName]]
    //    // Given the ID of a deleted character, this command restores that character to its owner.  (You can find the ID of a deleted character using the @finger command.)
    //    // If the deleted character's name has since been taken by a new character, you can specify a new name for the restored character as the second parameter.  (You can find if the name has been taken by also using the @finger command.)  Use a comma to separate the arguments.
    //    // If needed, you can specify an account name as a third parameter if the character should be restored to an account other than its original owner.  Again, use a comma between the arguments.
    //    // @bornagain - Restores a deleted character to an account.

    //    var hexNumber = parameters[0];

    //    if (hexNumber.StartsWith("0x"))
    //        hexNumber = hexNumber.Substring(2);

    //    if (hexNumber.EndsWith(","))
    //        hexNumber = hexNumber[..^1];

    //    if (uint.TryParse(hexNumber, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var existingCharIID))
    //    {
    //        var args = string.Join(" ", parameters);

    //        if (args.Contains(','))
    //        {
    //            var twoCommas = args.Count(c => c == ',') == 2;

    //            var names = string.Join(" ", parameters).Split(",");

    //            var newCharName = names[1].TrimStart(' ').TrimEnd(' ');

    //            if (newCharName.StartsWith("+"))
    //                newCharName = newCharName.Substring(1);
    //            newCharName = newCharName.First().ToString().ToUpper() + newCharName.Substring(1);

    //            string newAccountName;
    //            if (twoCommas)
    //            {
    //                newAccountName = names[2].TrimStart(' ').TrimEnd(' ').ToLower();

    //                var account = DatabaseManager.Authentication.GetAccountByName(newAccountName);

    //                if (account == null)
    //                {
    //                    //CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot restore. Account \"{newAccountName}\" is not in database.", ChatMessageType.Broadcast);
    //                    return;
    //                }

    //                if (PlayerManager.IsAccountAtMaxCharacterSlots(account.AccountName))
    //                {
    //                    //CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot restore. Account \"{newAccountName}\" has no free character slots.", ChatMessageType.Broadcast);
    //                    return;
    //                }

    //                DoCopyChar(session, $"0x{existingCharIID:X8}", existingCharIID, true, newCharName, account.AccountId);
    //            }
    //            else
    //            {
    //                if (PlayerManager.IsAccountAtMaxCharacterSlots(session.Player.Account.AccountName))
    //                {
    //                    //CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot restore. Account \"{session.Player.Account.AccountName}\" has no free character slots.", ChatMessageType.Broadcast);
    //                    return;
    //                }

    //                DoCopyChar(session, $"0x{existingCharIID:X8}", existingCharIID, true, newCharName);
    //            }
    //        }
    //        else
    //        {
    //            if (PlayerManager.IsAccountAtMaxCharacterSlots(session.Player.Account.AccountName))
    //            {
    //                //CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot restore. Account \"{session.Player.Account.AccountName}\" has no free character slots.", ChatMessageType.Broadcast);
    //                return;
    //            }

    //            DoCopyChar(session, $"0x{existingCharIID:X8}", existingCharIID, true);
    //        }
    //    }
    //    else
    //    {
    //        //CommandHandlerHelper.WriteOutputInfo(session, "Error, cannot restore. You must include an existing character id in hex form.\nExample: @copychar 0x500000AC\n         @copychar 0x500000AC, Newly Restored\n         @copychar 0x500000AC, Newly Restored, differentaccount\n", ChatMessageType.Broadcast);
    //    }
    //}

    [CommandHandler("load", AccessLevel.Developer, CommandHandlerFlag.None, -1)]
    public static void HandleLoad(Session session, params string[] parameters)
    {
        var name = parameters.Length > 0 ? parameters[0] : "Random";
        var id = PlayerManager.FindByName("Testvoid").Account.AccountId;

        DoLoad(name, id, PlayerSaveOptions.Default);

        //if (!string.Join(" ", parameters).Contains(','))
        //{
        //    //CommandHandlerHelper.WriteOutputInfo(session, "Error, cannot copy. You must include the existing character name followed by a comma and then the new name.\n Example: @copychar Old Name, New Name", ChatMessageType.Broadcast);
        //    return;
        //}

        //var names = string.Join(" ", parameters).Split(",");

        //var existingCharName = names[0].TrimStart(' ').TrimEnd(' ');
        //var newCharName = names[1].TrimStart(' ').TrimEnd(' ');

        //if (existingCharName.StartsWith("+"))
        //    existingCharName = existingCharName.Substring(1);
        //if (newCharName.StartsWith("+"))
        //    newCharName = newCharName.Substring(1);

        //newCharName = newCharName.First().ToString().ToUpper() + newCharName.Substring(1);

        //var existingPlayer = PlayerManager.FindByName(existingCharName);

        //if (existingPlayer == null || session.Characters.Count >= PropertyManager.GetLong("max_chars_per_account").Item)
        //{
        //    //CommandHandlerHelper.WriteOutputInfo(session, $"Failed to copy the character \"{existingCharName}\" to a new character \"{newCharName}\" for the account \"{session.Account}\"! Does the character exist? Is the new character name already taken, or is the account out of free character slots?", ChatMessageType.Broadcast);
        //    return;
        //}

        //DoCopyChar(session, existingCharName, existingPlayer.Guid.Full, false, newCharName);
    }

    public static void DoLoad(string newCharName, uint newAccountId, PlayerSaveOptions options)
    {
        var savePath = Directory.GetFiles(_saveDirectory).FirstOrDefault();

        var data = File.ReadAllBytes(savePath);
        var text = data.GZipToString();
        var save = JsonSerializer.Deserialize<PlayerSave>(text);

        ModManager.Log($"Loading {save.Character.Name} from {savePath}");

        //Copy Character template to a new account ID, name, and GUID 
        //Character existingCharacter = save.Character;
        var newCharacter = save.Character.CopyCharacterAs(newAccountId, newCharName, options);

        //Create a new biota for a player
        var newPlayerBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(save.PlayerBiota);

        //Clear as needed
        newPlayerBiota.Id = newCharacter.Id;
        newPlayerBiota.PropertiesAllegiance?.Clear();
        newPlayerBiota.HousePermissions?.Clear();

        var idSwaps = new ConcurrentDictionary<uint, uint>();
        //idSwaps[newPlayerBiota.Id] = newCharacter.Id;
        idSwaps[save.Character.Id] = newCharacter.Id;

        //Create temp entities used by /charcopy for swapping identities.
        //Todo: revisit when I have stuff samples to test with.  Logic here seems really weird.  Not sure a conversion is needed and want handling for other servers.
        //var tempInventory = save.Inventory.CreateEntityCopies(ref idSwaps);
        //var tempWielded = save.Wielded.CreateEntityCopies(ref idSwaps);

        //Copy possessions
        //var newInventoryItems = save.Inventory.ChangeOwner(newCharacter, tempInventory, idSwaps);
        //var newWieldedItems = save.Wielded.ChangeOwner(newCharacter, tempWielded, idSwaps);

        //var newTempWieldedItems = new List<ACE.Entity.Models.Biota>();
        //foreach (var item in items)
        //{
        //    //Todo, rethink this
        //    if (item.WeenieClassId == (uint)WeenieClassName.W_DEED_CLASS)
        //        continue;

        //    var newItemBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(item);
        //    var newGuid = GuidManager.NewDynamicGuid();
        //    idSwaps[newItemBiota.Id] = newGuid.Full;
        //    newItemBiota.Id = newGuid.Full;
        //    newTempWieldedItems.Add(newItemBiota);
        //}

        //return newTempWieldedItems;

        var newInventoryItems = options.IncludeInventory ? save.Inventory : new();
        newInventoryItems.ChangeOwner(newCharacter, ref idSwaps);
        
        var newWieldedItems = options.IncludeWielded ? save.Wielded : new();
        newWieldedItems.ChangeOwner(newCharacter, ref idSwaps);

        var ownerId = newCharacter.Id;
        var invCons = newInventoryItems.Select(x => x.GetProperty(PropertyInstanceId.Container) ?? 0);
        var wieldCons = newWieldedItems.Select(x => x.GetProperty(PropertyInstanceId.Container) ?? 0);


        Debugger.Break();
        //return;

        //Inventory and Wielded biotas are turned into WorldObjects with
        //WorldObjectFactory.CreateWorldObject -->
        //ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota -->
        //CreateWorldObject returning based on WeenieType

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
                //////CommandHandlerHelper.WriteOutputInfo(session, $"Failed to copy the character \"{(existingCharacter.IsPlussed ? "+" : "")}{existingCharacter.Name}\" to a new character \"{newPlayer.Name}\" for the account \"{newPlayer.Account.AccountName}\"! Does the character exist _AND_ is not currently logged in? Is the new character name already taken, or is the account out of free character slots?", ChatMessageType.Broadcast);
                ////CommandHandlerHelper.WriteOutputInfo(session, $"Failed to {(isDeletedChar ? "restore" : "copy")} the character \"{(existingCharacter.IsPlussed ? "+" : "")}{existingCharacter.Name}\" to a new character \"{newPlayer.Name}\" for the account \"{newPlayer.Account.AccountName}\"! Does the character exist? Is the new character name already taken, or is the account out of free character slots?", ChatMessageType.Broadcast);
                return;
            }

            PlayerManager.AddOfflinePlayer(newPlayer);

            //if (newAccountId == 0)
            //    session.Characters.Add(newPlayer.Character);
            //else
            //{
                var foundActiveSession = ACE.Server.Network.Managers.NetworkManager.Find(newAccountId);

                if (foundActiveSession != null)
                    foundActiveSession.Characters.Add(newPlayer.Character);
            //}

            //var msg = $"Successfully {(isDeletedChar ? "restored" : "copied")} the character \"{(existingCharacter.IsPlussed ? "+" : "")}{existingCharacter.Name}\" to a new character \"{newPlayer.Name}\" for the account \"{newPlayer.Account.AccountName}\".";
            ////CommandHandlerHelper.WriteOutputInfo(session, msg, ChatMessageType.Broadcast);
            //PlayerManager.BroadcastToAuditChannel(session.Player, msg);
        });
    }


    [CommandHandler("save", AccessLevel.Developer, CommandHandlerFlag.None, -1)]
    public static void HandleSave(Session session, params string[] parameters)
    {
        var player = PlayerManager.FindByName(parameters[0].Trim());

        if (player is null)
            return;

        DatabaseManager.Shard.GetCharacter(player.Guid.Full, character =>
        {
            if (character is null) return;

            DatabaseManager.Shard.GetPossessedBiotasInParallel(character.Id, possessions =>
            {
                var playerBiota = DatabaseManager.Shard.BaseDatabase.GetBiota(character.Id);
                DoSave(character, playerBiota, possessions);
            });
        });
    }

    #region Copy
    /// <summary>
    /// Create a save of a player from their Character, Biota, and possessions
    /// </summary>
    private static void DoSave(Character character, ACE.Database.Models.Shard.Biota playerBiota, PossessedBiotas possessions)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        
        //Todo: Do a proper BinaryWriter/Reader        
        //Create save
        var save = new PlayerSave
        {
            Character = character,
            PlayerBiota = playerBiota,
            Wielded = possessions.WieldedItems,
            Inventory = possessions.Inventory,
        };

        if (!Directory.Exists(_saveDirectory))
            Directory.CreateDirectory(_saveDirectory);

        var filePath = Path.Combine(_saveDirectory, $@".\{character.Name} - {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.acesave");

        var saveJson = JsonSerializer.Serialize(save, jsonOptions);
        var saveGZip = saveJson.ToGZip();
        
        File.WriteAllBytes(filePath, saveGZip);
    }


    #endregion

    private static void DoCopyChar(Session session, string existingCharName, uint existingCharId, bool isDeletedChar, string newCharacterName = null, uint newAccountId = 0)
    {
        DatabaseManager.Shard.GetCharacter(existingCharId, async existingCharacter =>
        {
            if (existingCharacter == null)
            {
                //CommandHandlerHelper.WriteOutputInfo(session, $"Failed to {(isDeletedChar ? "restore" : "copy")} the character \"{existingCharName}\" to a new character \"{newCharacterName}\" for the account \"{session.Account}\"! Does the character exist? Is the new character name already taken, or is the account out of free character slots?", ChatMessageType.Broadcast);
                return;
            }
            var newCharName = newCharacterName ?? existingCharacter.Name;

            var existingPlayerBiota = DatabaseManager.Shard.BaseDatabase.GetBiota(existingCharId);

            DatabaseManager.Shard.GetPossessedBiotasInParallel(existingCharId, existingPossessions =>
            {
                DatabaseManager.Shard.IsCharacterNameAvailable(newCharName, isAvailable =>
                {
                    if (!isAvailable)
                    {
                        ////CommandHandlerHelper.WriteOutputInfo(session, $"{newCharName} is not available to use for the {(isDeletedChar ? "restored" : "copied")} character name, try another name.", ChatMessageType.Broadcast);
                        return;
                    }
                    Debugger.Break();
                    var newPlayerGuid = GuidManager.NewPlayerGuid();

                    var newCharacter = new Character
                    {
                        Id = newPlayerGuid.Full,
                        AccountId = newAccountId > 0 ? newAccountId : session.Player.Account.AccountId,
                        Name = newCharName,
                        CharacterOptions1 = existingCharacter.CharacterOptions1,
                        CharacterOptions2 = existingCharacter.CharacterOptions2,
                        DefaultHairTexture = existingCharacter.DefaultHairTexture,
                        GameplayOptions = existingCharacter.GameplayOptions,
                        HairTexture = existingCharacter.HairTexture,
                        IsPlussed = existingCharacter.IsPlussed,
                        SpellbookFilters = existingCharacter.SpellbookFilters,
                        TotalLogins = 1 // existingCharacter.TotalLogins
                    };


                    foreach (var entry in existingCharacter.CharacterPropertiesContractRegistry)
                        newCharacter.CharacterPropertiesContractRegistry.Add(new CharacterPropertiesContractRegistry { CharacterId = newPlayerGuid.Full, ContractId = entry.ContractId, DeleteContract = entry.DeleteContract, SetAsDisplayContract = entry.SetAsDisplayContract });

                    foreach (var entry in existingCharacter.CharacterPropertiesFillCompBook)
                        newCharacter.CharacterPropertiesFillCompBook.Add(new CharacterPropertiesFillCompBook { CharacterId = newPlayerGuid.Full, QuantityToRebuy = entry.QuantityToRebuy, SpellComponentId = entry.SpellComponentId });

                    foreach (var entry in existingCharacter.CharacterPropertiesFriendList)
                        newCharacter.CharacterPropertiesFriendList.Add(new CharacterPropertiesFriendList { CharacterId = newPlayerGuid.Full, FriendId = entry.FriendId });

                    foreach (var entry in existingCharacter.CharacterPropertiesQuestRegistry)
                        newCharacter.CharacterPropertiesQuestRegistry.Add(new CharacterPropertiesQuestRegistry { CharacterId = newPlayerGuid.Full, LastTimeCompleted = entry.LastTimeCompleted, NumTimesCompleted = entry.NumTimesCompleted, QuestName = entry.QuestName });

                    foreach (var entry in existingCharacter.CharacterPropertiesShortcutBar)
                        newCharacter.CharacterPropertiesShortcutBar.Add(new CharacterPropertiesShortcutBar { CharacterId = newPlayerGuid.Full, ShortcutBarIndex = entry.ShortcutBarIndex, ShortcutObjectId = entry.ShortcutObjectId });

                    foreach (var entry in existingCharacter.CharacterPropertiesSpellBar)
                        newCharacter.CharacterPropertiesSpellBar.Add(new CharacterPropertiesSpellBar { CharacterId = newPlayerGuid.Full, SpellBarIndex = entry.SpellBarIndex, SpellBarNumber = entry.SpellBarNumber, SpellId = entry.SpellId });

                    foreach (var entry in existingCharacter.CharacterPropertiesSquelch)
                        newCharacter.CharacterPropertiesSquelch.Add(new CharacterPropertiesSquelch { CharacterId = newPlayerGuid.Full, SquelchAccountId = entry.SquelchAccountId, SquelchCharacterId = entry.SquelchCharacterId, Type = entry.Type });

                    foreach (var entry in existingCharacter.CharacterPropertiesTitleBook)
                        newCharacter.CharacterPropertiesTitleBook.Add(new CharacterPropertiesTitleBook { CharacterId = newPlayerGuid.Full, TitleId = entry.TitleId });

                    var idSwaps = new ConcurrentDictionary<uint, uint>();

                    var newPlayerBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(existingPlayerBiota);

                    idSwaps[newPlayerBiota.Id] = newPlayerGuid.Full;

                    newPlayerBiota.Id = newPlayerGuid.Full;
                    Debugger.Break();
                    if (newPlayerBiota.PropertiesAllegiance != null)
                        newPlayerBiota.PropertiesAllegiance.Clear();
                    if (newPlayerBiota.HousePermissions != null)
                        newPlayerBiota.HousePermissions.Clear();

                    var newTempWieldedItems = new List<Biota>();
                    foreach (var item in existingPossessions.WieldedItems)
                    {
                        var newItemBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(item);
                        var newGuid = GuidManager.NewDynamicGuid();
                        idSwaps[newItemBiota.Id] = newGuid.Full;
                        newItemBiota.Id = newGuid.Full;
                        newTempWieldedItems.Add(newItemBiota);
                    }

                    var newTempInventoryItems = new List<Biota>();
                    foreach (var item in existingPossessions.Inventory)
                    {
                        if (item.WeenieClassId == (uint)WeenieClassName.W_DEED_CLASS)
                            continue;

                        var newItemBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(item);
                        var newGuid = GuidManager.NewDynamicGuid();
                        idSwaps[newItemBiota.Id] = newGuid.Full;
                        newItemBiota.Id = newGuid.Full;
                        newTempInventoryItems.Add(newItemBiota);
                    }


                    var newWieldedItems = new List<ACE.Database.Models.Shard.Biota>();
                    foreach (var item in newTempWieldedItems)
                    {
                        if (item.PropertiesEnchantmentRegistry != null)
                        {
                            foreach (var entry in item.PropertiesEnchantmentRegistry)
                            {
                                if (idSwaps.ContainsKey(entry.CasterObjectId))
                                    entry.CasterObjectId = idSwaps[entry.CasterObjectId];
                            }
                        }

                        if (item.PropertiesIID != null)
                        {
                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.Owner, out var ownerId))
                            {
                                if (idSwaps.ContainsKey(ownerId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.Owner);
                                    item.PropertiesIID.Add(PropertyInstanceId.Owner, idSwaps[ownerId]);
                                }
                            }

                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.Wielder, out var wielderId))
                            {
                                if (idSwaps.ContainsKey(wielderId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.Wielder);
                                    item.PropertiesIID.Add(PropertyInstanceId.Wielder, idSwaps[wielderId]);
                                }
                            }

                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.AllowedActivator, out var allowedActivatorId))
                            {
                                if (idSwaps.ContainsKey(allowedActivatorId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.AllowedActivator);
                                    item.PropertiesIID.Add(PropertyInstanceId.AllowedActivator, idSwaps[allowedActivatorId]);

                                    item.PropertiesString.Remove(PropertyString.CraftsmanName);
                                    item.PropertiesString.Add(PropertyString.CraftsmanName, newCharName);
                                }
                            }

                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.AllowedWielder, out var allowedWielderId))
                            {
                                if (idSwaps.ContainsKey(allowedWielderId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.AllowedWielder);
                                    item.PropertiesIID.Add(PropertyInstanceId.AllowedWielder, idSwaps[allowedWielderId]);

                                    item.PropertiesString.Remove(PropertyString.CraftsmanName);
                                    item.PropertiesString.Add(PropertyString.CraftsmanName, newCharName);
                                }
                            }
                        }

                        newWieldedItems.Add(ACE.Database.Adapter.BiotaConverter.ConvertFromEntityBiota(item));
                    }

                    var newInventoryItems = new List<ACE.Database.Models.Shard.Biota>();
                    foreach (var item in newTempInventoryItems)
                    {
                        if (item.PropertiesEnchantmentRegistry != null)
                        {
                            foreach (var entry in item.PropertiesEnchantmentRegistry)
                            {
                                if (idSwaps.ContainsKey(entry.CasterObjectId))
                                    entry.CasterObjectId = idSwaps[entry.CasterObjectId];
                            }
                        }

                        if (item.PropertiesIID != null)
                        {
                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.Owner, out var ownerId))
                            {
                                if (idSwaps.ContainsKey(ownerId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.Owner);
                                    item.PropertiesIID.Add(PropertyInstanceId.Owner, idSwaps[ownerId]);
                                }
                            }

                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.Container, out var containerId))
                            {
                                if (idSwaps.ContainsKey(containerId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.Container);
                                    item.PropertiesIID.Add(PropertyInstanceId.Container, idSwaps[containerId]);
                                }
                            }

                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.AllowedActivator, out var allowedActivatorId))
                            {
                                if (idSwaps.ContainsKey(allowedActivatorId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.AllowedActivator);
                                    item.PropertiesIID.Add(PropertyInstanceId.AllowedActivator, idSwaps[allowedActivatorId]);

                                    item.PropertiesString.Remove(PropertyString.CraftsmanName);
                                    item.PropertiesString.Add(PropertyString.CraftsmanName, $"{(existingCharacter.IsPlussed ? "+" : "")}{newCharName}");
                                }
                            }

                            if (item.PropertiesIID.TryGetValue(PropertyInstanceId.AllowedWielder, out var allowedWielderId))
                            {
                                if (idSwaps.ContainsKey(allowedWielderId))
                                {
                                    item.PropertiesIID.Remove(PropertyInstanceId.AllowedWielder);
                                    item.PropertiesIID.Add(PropertyInstanceId.AllowedWielder, idSwaps[allowedWielderId]);

                                    item.PropertiesString.Remove(PropertyString.CraftsmanName);
                                    item.PropertiesString.Add(PropertyString.CraftsmanName, $"{(existingCharacter.IsPlussed ? "+" : "")}{newCharName}");
                                }
                            }
                        }

                        newInventoryItems.Add(ACE.Database.Adapter.BiotaConverter.ConvertFromEntityBiota(item));
                    }

                    Player newPlayer;
                    if (newPlayerBiota.WeenieType == WeenieType.Admin)
                        newPlayer = new Admin(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);
                    else if (newPlayerBiota.WeenieType == WeenieType.Sentinel)
                        newPlayer = new Sentinel(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);
                    else
                        newPlayer = new Player(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);

                    newPlayer.Name = newCharName;
                    newPlayer.ChangesDetected = true;
                    newPlayer.CharacterChangesDetected = true;

                    newPlayer.Allegiance = null;
                    newPlayer.AllegianceOfficerRank = null;
                    newPlayer.MonarchId = null;
                    newPlayer.PatronId = null;
                    newPlayer.HouseId = null;
                    newPlayer.HouseInstance = null;

                    if (newPlayer.Character.CharacterPropertiesShortcutBar != null)
                    {
                        foreach (var entry in newPlayer.Character.CharacterPropertiesShortcutBar)
                        {
                            if (idSwaps.ContainsKey(entry.ShortcutObjectId))
                                entry.ShortcutObjectId = idSwaps[entry.ShortcutObjectId];
                        }
                    }

                    if (newPlayer.Biota.PropertiesEnchantmentRegistry != null)
                    {
                        foreach (var entry in newPlayer.Biota.PropertiesEnchantmentRegistry)
                        {
                            if (idSwaps.ContainsKey(entry.CasterObjectId))
                                entry.CasterObjectId = idSwaps[entry.CasterObjectId];
                        }
                    }

                    var possessions = newPlayer.GetAllPossessions();
                    var possessedBiotas = new Collection<(Biota biota, ReaderWriterLockSlim rwLock)>();
                    foreach (var possession in possessions)
                        possessedBiotas.Add((possession.Biota, possession.BiotaDatabaseLock));

                    // We must await here -- 
                    DatabaseManager.Shard.AddCharacterInParallel(newPlayer.Biota, newPlayer.BiotaDatabaseLock, possessedBiotas, newPlayer.Character, newPlayer.CharacterDatabaseLock, saveSuccess =>
                    {
                        if (!saveSuccess)
                        {
                            //////CommandHandlerHelper.WriteOutputInfo(session, $"Failed to copy the character \"{(existingCharacter.IsPlussed ? "+" : "")}{existingCharacter.Name}\" to a new character \"{newPlayer.Name}\" for the account \"{newPlayer.Account.AccountName}\"! Does the character exist _AND_ is not currently logged in? Is the new character name already taken, or is the account out of free character slots?", ChatMessageType.Broadcast);
                            ////CommandHandlerHelper.WriteOutputInfo(session, $"Failed to {(isDeletedChar ? "restore" : "copy")} the character \"{(existingCharacter.IsPlussed ? "+" : "")}{existingCharacter.Name}\" to a new character \"{newPlayer.Name}\" for the account \"{newPlayer.Account.AccountName}\"! Does the character exist? Is the new character name already taken, or is the account out of free character slots?", ChatMessageType.Broadcast);
                            return;
                        }

                        PlayerManager.AddOfflinePlayer(newPlayer);

                        if (newAccountId == 0)
                            session.Characters.Add(newPlayer.Character);
                        else
                        {
                            var foundActiveSession = ACE.Server.Network.Managers.NetworkManager.Find(newAccountId);

                            if (foundActiveSession != null)
                                foundActiveSession.Characters.Add(newPlayer.Character);
                        }

                        var msg = $"Successfully {(isDeletedChar ? "restored" : "copied")} the character \"{(existingCharacter.IsPlussed ? "+" : "")}{existingCharacter.Name}\" to a new character \"{newPlayer.Name}\" for the account \"{newPlayer.Account.AccountName}\".";
                        ////CommandHandlerHelper.WriteOutputInfo(session, msg, ChatMessageType.Broadcast);
                        PlayerManager.BroadcastToAuditChannel(session.Player, msg);
                    });
                });
            });

        });
    }

}
