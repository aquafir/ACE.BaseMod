using ACE.DatLoader.Entity;
using ACE.Server.Command.Handlers;

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
#if REALM
        wo.SetProperty(Settings.EarSourcePlayerProp, killedPlayer.Guid.ClientGUID);
#else
        wo.SetProperty(Settings.EarSourcePlayerProp, killedPlayer.Guid.Full);
# endif
        killedPlayer.SetProperty(Settings.LastEarDropProp, current);

        __result.Add(wo);
    }



    [CommandHandler("tpk", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandlePk(Session session, params string[] parameters)
    {
        if (session.Player is not Player p)
            return;

        //Tries to set pk status
        if (p.PlayerKillerStatus != PlayerKillerStatus.NPK)
        {
            if (p.PKTimerActive)
            {
                p.SendMessage($"Unable to become NPK in combat.");
                return;
            }

            p.PlayerKillerStatus = PlayerKillerStatus.NPK;
            p.PkLevel = PKLevel.NPK;
            p.SendMessage($"You are now NPK");
        }
        else
        {
            //var lastPk = p.GetProperty(Settings.LastPkTimestamp) ?? 0;
            var current = Time.GetUnixTime();
            var lapsed = current - p.LastPkAttackTimestamp; 

            if(lapsed < Settings.SecondsBetweenPk)
            {
                p.SendMessage($"You last attacked someone {lapsed:N0} seconds ago.  {Settings.SecondsBetweenPk} seconds must pass before you can become a PK again.");
                return;
            }

            p.PlayerKillerStatus = PlayerKillerStatus.PK;
            p.PkLevel = PKLevel.PK;
            p.SendMessage($"You are now PK");
        }

        session.Player.EnqueueBroadcast(new GameMessagePublicUpdatePropertyInt(session.Player, PropertyInt.PlayerKillerStatus, (int)session.Player.PlayerKillerStatus));
    }
}
