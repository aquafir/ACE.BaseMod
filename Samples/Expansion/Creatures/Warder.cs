using static ACE.Server.WorldObjects.Player;

namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Warder))]
public class Warder : CreatureEx
{
    public Warder(Biota biota) : base(biota) { }
#if REALM
    public Warder(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Warder(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Warding " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

    }

    const float range = 8;
    const int candidates = 5;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.CreatePlayerSpell), new Type[] { typeof(WorldObject), typeof(TargetCategory), typeof(uint), typeof(WorldObject) })]
    private static bool CreatePlayerSpell(WorldObject target, TargetCategory targetCategory, ref uint spellId, WorldObject casterItem, ref Player __instance)
    {
        //Are warders always valid targets?
        if (target is Warder)
            return true;

        //Could go either way with the target being the warded area or the player
        if (!__instance.GetSplashTargets(target, TargetExclusionFilter.OnlyCreature, range).Any(x => x is Warder w))
            return true;

        target.PlayAnimation(PlayScript.RestrictionEffectBlue);
        __instance.SendMessage($"The target of your spell is under a ward.");
        __instance.FailCast(false);

        return false;
    }
}