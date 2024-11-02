//using ACE.Server.Entity.Actions;

//namespace Tower;

//[CommandCategory(nameof(Feature.AutoBattle))]
//[HarmonyPatchCategory(nameof(Feature.AutoBattle))]
//public static class AutoBattle
//{
//    static PropertyBool Battling = (PropertyBool)50505;
//    static PropertyInstanceId AutoTarget = (PropertyInstanceId)50505;
//    public static bool InBattle(this Player player) => player.GetProperty(Battling) == true;

//    public static bool TryGetTarget(this Player player, out WorldObject target)
//    {
//        target = null;

//        if (player.selectedTarget is null)
//            return false;

//        target = player.selectedTarget.TryGetWorldObject();
//        return target is not null;
//    }
//    public static bool TryGetTarget(this Player player, out Creature target)
//    {
//        target = null;

//        if (player.selectedTarget is null)
//            return false;

//        target = player.selectedTarget.TryGetWorldObject() as Creature;
//        return target is not null;
//    }

//    [CommandHandler("t2", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
//    public static void HandleAutobattle(Session session, params string[] parameters)
//    {
//        var player = session.Player;

//        var target = player.MeleeTarget ?? player.selectedTarget?.TryGetWorldObject();
//        if (target is null || target.WeenieType != WeenieType.Creature)
//        {
//            player.SendMessage($"Nothing selected.");
//            return;
//        }
//        else
//            player.SendMessage($"Battling {target.Name}.");

//        player.PrevSettings.Animation = Settings.Animation.Invalid;
//        player.AttackHeight = AttackHeight.Low;
//        //player.
//        //var time = player.SimulateSwingMotion(target, out var frames);
//        //player.SendMessage($"{time} - {frames.Count} swings");
//        //        player


//        Auto(player);
//    }

//    private static void Auto(Player player)
//    {
//        var actionChain = new ActionChain();
//        float time = 2;
//        actionChain.AddAction(player, () =>
//        {
//            var nearest = player.GetSplashTargets(player, TargetExclusionFilter.OnlyCreature, 5).FirstOrDefault();

//            if (nearest is null)
//            {
//                player.SendMessage("Nothing nearby.");
//                return;
//            }

//            time = player.SimulateSwingMotion(nearest, out var frames);
//            player.DamageTarget(nearest, player);

//            actionChain.AddDelaySeconds(time);
//            actionChain.AddAction(player, () => Auto(player));
//            player.SendMessage($"{nearest?.Name} nearby, delay {time} seconds");
//        });
//        actionChain.EnqueueChain();
//    }

//    [HarmonyPostfix]
//    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTargetSelected), new Type[] { typeof(Player) })]
//    public static void PostOnTargetSelected(Player player, ref Creature __instance, ref bool __result)
//    {
//        if (player.InBattle())
//            return;

//        //player.SendMessage("Selected  {__instance?.Name}");
//        if (player.CombatMode == CombatMode.Melee)
//        {
//            player.SendMessage($"Battling {__instance?.Name}");
//            player.SetProperty(Battling, true);
//            player.SetProperty(AutoTarget, player.selectedTarget.Guid.Full);
//        }

//        //var actionChain = new ActionChain();

//        //actionChain.AddLoop(player, () =>
//        //{
//        //    //origins[0] += System.Numerics.Vector3.UnitZ;
//        //    origins[0] += new Vector3(ThreadSafeRandom.Next(0, perturbance), ThreadSafeRandom.Next(0, perturbance), ThreadSafeRandom.Next(0, perturbance));
//        //    velocity = Vector3.Multiply(CalculateProjectileVelocity(spell, target, spellType, origins[0]), (float)ThreadSafeRandom.Next(lowVelocity, highVelocity));
//        //    LaunchSpellProjectiles(spell, target, spellType, null, false, false, origins, velocity);
//        //});

//        //actionChain.AddLoop(player, () => true, () =>
//        //{

//        //})
//        //actionChain.AddDelaySeconds(7);
//        //actionChain.AddAction(__instance, () => __instance.GiveOfflineProgress());
//        //actionChain.EnqueueChain();
//    }

//    [HarmonyPostfix]
//    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTargetDeselected), new Type[] { typeof(Player) })]
//    public static void PostOnTargetDeselected(Player player, ref Creature __instance, ref bool __result)
//    {
//        if (!player.InBattle())
//            return;

//        //if(!player.TryGetTarget();

//        player.SendMessage($"Leaving battle");
//        player.SetProperty(Battling, false);
//        player.RemoveProperty(AutoTarget);
//    }


//    [HarmonyPrefix]
//    [HarmonyPatch(typeof(Player), nameof(Player.Heartbeat), new Type[] { typeof(double) })]
//    public static void PreHeartbeat(double currentUnixTime, ref Player __instance)
//    {
//        //__instance.SendMessage($"{__instance.InBattle()} - {__instance.TryGetTarget(out var c)} ");
//        if (!__instance.InBattle())
//            return;

//        if (!__instance.TryGetTarget(out Creature creature))
//            return;

//        __instance.SendMessage($"Attacking {creature.Name}");
//        //if(player.dist)
//        //__instance.HandleActionTargetedMeleeAttack(creature.Guid.Full, (uint)(__instance.AttackHeight ?? AttackHeight.Low), __instance.PowerLevel);
//        __instance.MeleeTarget = creature;
//        __instance.Attack(creature, __instance.AttackSequence);
//    }
//}
