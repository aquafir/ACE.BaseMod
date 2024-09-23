namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Merger))]
public class Merger : CreatureEx
{
    public Merger(Biota biota) : base(biota) { }
#if REALM
    public Merger(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Merger(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Assimilating " + Name;
    }

    //Custom behavior
    const int interval = 3;
    private int count = interval;

    const int maxMerges = 5;
    int merges = 0;
    float mergeBoost = 1.1f;
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (count-- > 0)
            return;

        if (AttackTarget is not Player p || merges >= maxMerges)
            return;

        //Get something to merge with of the same wcid but not a merger?
        var mergeTarget = p.GetSplashTargets(this, TargetExclusionFilter.OnlyCreature, 10).Take(5)
            .Where(x => x.WeenieClassId == WeenieClassId && x is not Merger)
            .FirstOrDefault();

        if (mergeTarget is null) return;

        //Scale stats
        this.ScaleAttributeBase(mergeBoost, Enum.GetValues<PropertyAttribute>());

        //Add health
        Health.StartingValue += mergeTarget.Health.Current;
        Health.Current += mergeTarget.Health.Current;
        DamageHistory.OnHeal(mergeTarget.Health.Current);

        //Increase xp?
        XpOverride += Weenie.GetProperty(PropertyInt.XpOverride);

        mergeTarget.OnDeath();
        mergeTarget.Die();
        merges++;

        count = interval;

        p.SendMessage($"{Name} has merged {merges} times.");
    }
}