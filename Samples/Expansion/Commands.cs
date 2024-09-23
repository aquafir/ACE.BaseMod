//using Discord;

namespace Expansion;
public static class Commands
{
    [CommandHandler("sim", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void Sim(Session session, params string[] parameters)
    {
        var player = session.Player;



        //var mod = ModManager.GetModContainerByName(nameof(Discord));
        //if (mod is null)
        //{
        //    ModManager.Log($"Host not found.");
        //    return;
        //}
        //var relay = Discord.PatchClass.DiscordRelay;
        //if (mod.Instance is InterModHost.Mod hostMod && hostMod.Patch is InterModHost.HostPatchClass host)
        //    ModManager.Log($"Found host: {host.InstanceCounter++}");

        //relay.QueueMessageForDiscord("Hello?");

        //var profiles = player.CurrentLandblock.GetCreatureProfiles();

        //var sb = new StringBuilder("\n");

        ////DatabaseManager.World.GetCachedWeenie(weenieClassId)
        ////Base on spawned?
        ////foreach (var g in gens)
        ////{
        ////    foreach (var spawns in g.GeneratorProfiles.GroupBy(x => x.WeenieClassId))
        ////    {
        ////        DatabaseManager.World.GetCachedWeenie(spawns.Key);
        ////        //WorldObjectFactory.CreateNewWorldObject
        ////        //profile.Biota.ToString
        ////        //profile.Biota.WeenieClassId
        ////    }
        ////}


        //foreach (var spawns in profiles.GroupBy(x => x.WeenieClassId))
        //{
        //    var weenie = DatabaseManager.World.GetCachedWeenie(spawns.Key);

        //    if (weenie is null || weenie.WeenieType != WeenieType.Creature || weenie.IsNpc())
        //        continue;

        //    sb.Append($"{spawns.Count(),-8}{weenie.GetName()}\n");

        //    var wo = WorldObjectFactory.CreateNewWorldObject(weenie, null) as Creature;
        //    if (wo is null)
        //        continue;

        //    var de = player.SimulateDamage(wo, player);
        //    de.ShowInfo(player);
        //    sb.Append($"  Evasion: {de.EvasionChance:P3}\n");
        //    sb.Append($"  PreMit: {de.DamageBeforeMitigation:0.00)}\n");
        //    sb.Append($"  Damage: {de.Damage:0.00)}\n");
        //    sb.Append($"  Weapon: {de.Weapon?.Name}\n");
        //    //sb.Append($"  Damage: {de.Damage}\n");
        //    //WorldObjectFactory.CreateNewWorldObject
        //    //profile.Biota.ToString
        //    //profile.Biota.WeenieClassId
        //}

        //player.SendMessage($"{sb}");
        ////player.CurrentLandblock.RespawnCreatures
    }

    [CommandHandler("t2", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void T2(Session session, params string[] parameters)
    {
        var p = session.Player;
        var s = p.selectedTarget?.TryGetWorldObject();
        if (p is null || s is null || s is not Creature c)
            return;

        Debugger.Break();
        c.TakeDamage(null, DamageType.Undef, 10);
    }

    [CommandHandler("t1", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void T1(Session session, params string[] parameters)
    {
        var player = session.Player;
        var selected = player.selectedTarget?.TryGetWorldObject();
        if (player is null || selected is null)
            return;

        selected.PlayAnimation(PlayScript.AttribDownGreen);

        var t = player.GetSplitTargets(selected, TargetExclusionFilter.OnlyPlayer, 120f, 30)?.TakeWhile(x => x.Guid != selected.Guid).ToList();
        if (t is null)
            return;

        foreach (var item in t)
        {
            item.PlayAnimation(PlayScript.AttribUpBlue);
        }

        var msg = "\nTargets:\n" + string.Join("\n", t.Select(x => x.Name));
        player.SendMessage(msg);
        //player.PrevMotionCommand = MotionCommand.Invalid;
        //player.AttackHeight = AttackHeight.Low;
        //player.T4(t, out var frames);
    }

    public static float T4(this Player player, WorldObject target, out List<(float time, AttackHook attackHook)> attackFrames)
    {
        var baseSpeed = player.GetAnimSpeed();
        var animSpeedMod = player.IsDualWieldAttack ? 1.2f : 1.0f;     // dual wield swing animation 20% faster
        var animSpeed = baseSpeed * animSpeedMod;

        var swingAnimation = player.GetSwingAnimation();
        var animLength = ACE.Server.Physics.Animation.MotionTable.GetAnimationLength(player.MotionTableId, player.CurrentMotionState.Stance, swingAnimation, animSpeed);
        attackFrames = ACE.Server.Physics.Animation.MotionTable.GetAttackFrames(player.MotionTableId, player.CurrentMotionState.Stance, swingAnimation);

        // broadcast player swing animation to clients
        var motion = new Motion(player, swingAnimation, animSpeed);
        if (PropertyManager.GetBool("persist_movement").Item)
        {
            motion.Persist(player.CurrentMotionState);
        }
        motion.MotionState.TurnSpeed = 2.25f;
        //motion.MotionFlags |= MotionFlags.StickToObject;
        motion.TargetGuid = target.Guid;
        player.CurrentMotionState = motion;

        for (var i = 0; i < 20; i++)
            player.EnqueueBroadcastMotion(motion);

        if (player.FastTick)
            player.PhysicsObj.stick_to_object(target.Guid.Full);

        return animLength;
    }


    //Todo: remove in release
    [CommandHandler("hp", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HP(Session session, params string[] parameters)
    {
        // @delete - Deletes the selected object. Players may not be deleted this way.

        var player = session.Player;
        player.Vitals[PropertyAttribute2nd.MaxHealth].Ranks = 100000000;
        player.SetMaxVitals();
    }

    //[CommandHandler("clean", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    //public static void Clean(Session session, params string[] parameters)
    //{
    //    var player = session.Player;

    //    try
    //    {
    //        foreach (var item in player.Inventory.Values)
    //        {
    //            //player.TryRemoveFromInventoryWithNetworking(item.Key, out var i, Player.RemoveFromInventoryAction.None);
    //            //player.Session.Network.EnqueueSend(new GameMessageInventoryRemoveObject(i));
    //            player.DeleteItem(item);
    //        }

    //        foreach (var item in player.EquippedObjects.Values)
    //        {
    //            //player.TryRemoveFromInventoryWithNetworking(item.Key, out var i, Player.RemoveFromInventoryAction.None);
    //            //player.Session.Network.EnqueueSend(new GameMessageInventoryRemoveObject(i));
    //            player.DeleteItem(item);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ModManager.Log($"{ex.Message}", ModManager.LogLevel.Error);
    //    }
    //}
    [CommandHandler("clean", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void Clean(Session session, params string[] parameters)
    {
        // @delete - Deletes the selected object. Players may not be deleted this way.

        var player = session.Player;

        foreach (var item in player.Inventory.Values)
        {
            if (item is Container)
                continue;

            item.DeleteObject(player);
            session.Network.EnqueueSend(new GameMessageDeleteObject(item));
        }
    }

}
