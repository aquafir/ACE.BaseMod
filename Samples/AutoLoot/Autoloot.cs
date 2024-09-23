using AutoLoot.Lib.VTClassic;
using System.Collections.Concurrent;

namespace AutoLoot;

[HarmonyPatch]
public class AutoLoot
{
    static readonly ConcurrentDictionary<Player, LootCore> lootProfiles = new();
    //Dump tests here
    [CommandHandler("loot", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, -1)]
#if REALM
public static void HandleLoadProfile(ISession session, params string[] parameters)
#else
    public static void HandleLoadProfile(Session session, params string[] parameters)
#endif
    {
        var player = session.Player;

        try
        {
            //Make the path if it doesn't exist
            Directory.CreateDirectory(PatchClass.Settings.LootProfilePath);
            var profiles = Directory.GetFiles(PatchClass.Settings.LootProfilePath, "*.utl", SearchOption.AllDirectories);

            //Print
            var sb = new StringBuilder("\nProfiles:");
            if (parameters.Length == 0)
            {
                for (var i = 0; i < profiles.Length; i++)
                {
                    var profilePath = profiles[i];
                    var fi = new FileInfo(profilePath);
                    sb.Append($"  \n{i}) {fi.Name}  ==  {fi.Length / 1024:0}kb");
                }
                player.SendMessage(sb.ToString());
                return;
            }

            //Select index
            var selected = "";
            if (uint.TryParse(parameters[0], out var index) && index < profiles.Length)
            {
                selected = profiles[index];
            }
            else
            {
                var joined = string.Join(' ', parameters);
                selected = profiles.Where(x => x.Contains(joined, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(selected) || !File.Exists(selected))
            {
                player.SendMessage($"No loot profile selected.");

                //Clear bad profile
                if (lootProfiles.TryGetValue(player, out var oldProfile))
                    oldProfile.Shutdown();

                return;
            }

            //No profile loaded
            if (!lootProfiles.TryGetValue(player, out var profile))
            {
                profile = new();
                profile.Startup();
                profile.LoadProfile(selected, false);
                lootProfiles.TryAdd(player, profile);
            }
            else
            {
                profile.Shutdown();
                profile.LoadProfile(selected, false);
            }

            player.SendMessage($"Loaded profile: {selected}");
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
            player.SendMessage($"Failed to load loot profile!");
        }
    }



    #region Patches
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, ref Creature __instance, ref List<WorldObject> __result)
    {
        if (killer.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        if (!lootProfiles.TryGetValue(player, out var profile))
            return;

        try
        {
            var watch = Stopwatch.StartNew();
            var sb = new StringBuilder();

            var items = corpse.Inventory.Values.ToList();

            int looted = 0;
            foreach (var item in items)
            {
                var lootAction = profile.GetLootDecision(item, player);

                if (lootAction.IsNoLoot)
                    continue;

                looted++;
                sb.Append($"\n {item.Name} matches {lootAction._lootAction} rule {lootAction.RuleName}");

                player.TryCreateInInventoryWithNetworking(item);
            }
            if (looted == 0)
                return;

            watch.Stop();
            sb.Append($"\n=====Looted {looted}/{items.Count} items in {watch.ElapsedMilliseconds} ms=====");

            player.SendMessage(sb.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }
    #endregion
}
