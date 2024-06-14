using ACE.Server.WorldObjects;

namespace Tower;

[HarmonyPatchCategory(nameof(AutoBattle))]
public static class AutoBattle
{
    static PropertyBool Battling = (PropertyBool)50505;
    static PropertyInstanceId AutoTarget = (PropertyInstanceId)50505;
    public static bool InBattle(this Player player) => player.GetProperty(Battling) == true;

    public static bool TryGetTarget(this Player player, out WorldObject target) {
        target = null;

        if (player.selectedTarget is null)
            return false;

        target = player.selectedTarget.TryGetWorldObject();
        return target is not null;
    }
    public static bool TryGetTarget(this Player player, out Creature target)
    {
        target = null;

        if (player.selectedTarget is null)
            return false;

        target = player.selectedTarget.TryGetWorldObject() as Creature;
        return target is not null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTargetSelected), new Type[] { typeof(Player) })]
    public static void PostOnTargetSelected(Player player, ref Creature __instance, ref bool __result)
    {
        if (player.InBattle())
            return;

        //player.SendMessage("Selected  {__instance?.Name}");
        if (player.CombatMode == CombatMode.Melee)
        {
            player.SendMessage($"Battling {__instance?.Name}");
            player.SetProperty(Battling, true);
            player.SetProperty(AutoTarget, player.selectedTarget.Guid.Full);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTargetDeselected), new Type[] { typeof(Player) })]
    public static void PostOnTargetDeselected(Player player, ref Creature __instance, ref bool __result)
    {
        if (!player.InBattle())
            return;

        //if(!player.TryGetTarget();

        player.SendMessage($"Leaving battle");
        player.SetProperty(Battling, false);
        player.RemoveProperty(AutoTarget);
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.Heartbeat), new Type[] { typeof(double) })]
    public static void PreHeartbeat(double currentUnixTime, ref Player __instance)
    {
        //__instance.SendMessage($"{__instance.InBattle()} - {__instance.TryGetTarget(out var c)} ");
        if (!__instance.InBattle())
            return;

        if (!__instance.TryGetTarget(out Creature creature))
            return;

        __instance.SendMessage($"Attacking {creature.Name}");
        //if(player.dist)
        //__instance.HandleActionTargetedMeleeAttack(creature.Guid.Full, (uint)(__instance.AttackHeight ?? AttackHeight.Low), __instance.PowerLevel);
        __instance.MeleeTarget = creature;
        __instance.Attack(creature, __instance.AttackSequence);
    }


    [CommandHandler("t3", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
#if REALM
    public static void HandleT3(ISession session, params string[] parameters)
#else
public static void HandleT3(Session session, params string[] parameters)
#endif
    {
        var player = session.Player;
        PlayerFactoryEx.AddAllSpells(player);
    }
}
