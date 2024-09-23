namespace Tower;

[CommandCategory(nameof(Feature.AetheriaLoot))]
[HarmonyPatchCategory(nameof(Feature.AetheriaLoot))]
public static class AetheriaLoot
{
    static AetheriaSettings Settings => PatchClass.Settings.Aetheria;
    static PropertyDataId GroupProp => Settings.AetheriaLootGroupProperty;

    private static bool TryGetAetheriaDrops(this WorldObject wo, out AetheriaDrop aetheriaDrops)
    {
        aetheriaDrops = default;

        if (wo.GetProperty(GroupProp) is not uint group || !Settings.Groups.TryGetValue(group, out aetheriaDrops))
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AutoLoot), nameof(AutoLoot.GetLoot), new Type[] { typeof(Player), typeof(Creature) })]
    public static void PostGetLoot(Player player, Creature creature, ref List<WorldObject> __result)
    {
        //Adds a roll for aetheria in Tower's loot generation
        if (creature is null || !creature.TryGetAetheriaDrops(out var drops))
            return;

        var aetheria = drops.RollAetheria();

        if (aetheria is not null)
            __result.Add(aetheria);
    }

    private static WorldObject RollAetheria(this AetheriaDrop drops)
    {
        if (ThreadSafeRandom.Next(0.0f, 1.0f) > drops.Odds)
            return null;

        //Create aetheria
        uint aetheriaType = drops.Color switch
        {
            AetheriaColor.Blue => ACE.Server.Entity.Aetheria.AetheriaBlue,
            AetheriaColor.Yellow => ACE.Server.Entity.Aetheria.AetheriaYellow,
            AetheriaColor.Red => ACE.Server.Entity.Aetheria.AetheriaRed,
            _ => ACE.Server.Entity.Aetheria.AetheriaBlue,
        };

        var wo = WorldObjectFactory.CreateNewWorldObject(aetheriaType) as Gem;

        //if (wo is null)
        //    return;

        //Roll level 1-5
        wo.ItemMaxLevel = Math.Clamp(ThreadSafeRandom.Next(drops.LevelMin, drops.LevelMax), 1, 5);
        wo.IconOverlayId = wo.ItemMaxLevel.Value switch
        {
            1 => 0x6006C34,
            2 => 0x6006C35,
            3 => 0x6006C36,
            4 => 0x6006C37,
            5 => 0x6006C38,
            _ => 0x6006C34,
        };

        return wo;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryUseItem), new Type[] { typeof(WorldObject), typeof(bool) })]
    public static bool PreTryUseItem(WorldObject item, bool success, ref Player __instance)
    {
        if (item is null || !item.TryGetAetheriaDrops(out var drops))
            return true;

        //Take the object
        if (!__instance.TryConsumeFromInventoryWithNetworking(item.WeenieClassId, 1))
        {
            __instance.SendMessage("Failed to make Aetheria.");
        }
        else
        {
            //Roll ateheria
            var aetheria = drops.RollAetheria();

            //Successfully added
            if (aetheria is not null && __instance.TryCreateInInventoryWithNetworking(aetheria))
                __instance.SendMessage("Created Aetheria!");
            else
                __instance.SendMessage("Better luck next time!");
        }
        __instance.SendUseDoneEvent();

        return false;
    }
}
