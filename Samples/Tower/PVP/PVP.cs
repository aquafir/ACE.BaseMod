﻿using ACE.DatLoader.Entity;

namespace Tower;

[CommandCategory(nameof(Feature.PVP))]
[HarmonyPatchCategory(nameof(Feature.PVP))]
public static class PVP
{
    static PVPSettings Settings => PatchClass.Settings.PVP;

    public static double LastEarDrop(this Player player) =>
        player.GetProperty(Settings.LastEarDropProp) ?? 0;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AutoLoot), nameof(AutoLoot.GetLoot), new Type[] { typeof(Player), typeof(Creature) })]
    public static void PostGetLoot(Player player, Creature creature, ref List<WorldObject> __result)
    {
        //Adds a roll for PVP in Tower's loot generation
        if (creature is null || creature is not Player killedPlayer)
            return;

        //Check level difference
        //Todo: decide on absolute or levels beneath killer
        var levelDif = player.Level - killedPlayer.Level;

        if (levelDif > Settings.MaxLevelDifference)
        {
            player.SendMessage($"No trophies given for kills of players {Settings.MaxLevelDifference} beneath you.");
            return;
        }

        //Check time
        var time = killedPlayer.LastEarDrop();
        var current = Time.GetUnixTime();
        var delta = current - time;

        if (delta < Settings.SecondsBetweenDrops)
        {
            player.SendMessage($"No trophies given for kills of players killed within {Settings.SecondsBetweenDrops} seconds of their last trophy.");
            return;
        }

        //Make an ear
        var wo = WorldObjectFactory.CreateNewWorldObject(Settings.EarId);
        if (wo is null) 
            return;

        //Set values
        wo.Name = $"{killedPlayer.Name}'s Ear";
        wo.LongDesc = $"PK trophy from when {player.Name} ({player.Level}) killed {killedPlayer.Name} ({killedPlayer.Level})";
        wo.SetProperty(Settings.EarSourcePlayerProp, killedPlayer.Guid.ClientGUID);
        killedPlayer.SetProperty(Settings.LastEarDropProp, current);

        __result.Add(wo);
    }
}