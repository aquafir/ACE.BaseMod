namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Berserker))]
public class Berserker : CreatureEx
{
    public Berserker(Biota biota) : base(biota) { }
#if REALM
    public Berserker(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Berserker(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Raging " + Name;
    }

    //Custom behavior
    const float scaleCap = 1.3f;
    const float scaleDelta = scaleCap - 1;
    const float threshold = .6f;
    float scale = 1f;
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        //Enrage only once?
        if (scale != 1f) return;

        var hp = this.PercentHealth();
        if (hp > threshold) return;

        //Linear scaling
        scale += scaleDelta * this.PercentHealth();

        if (AttackTarget is Player p)
            p.SendMessage($"{Name} is enraged... it grows by {scale}.");

        ScaleBy(scale);
    }

    private void ScaleBy(float amount)
    {
        //Todo: send updates?
        var attributesToScale = Enum.GetValues<PropertyAttribute>();
        this.ScaleAttributeBase(amount, attributesToScale);
    }

    public override string GetAttackMessage(Creature creature, DamageType damageType, uint amount)
    {
        return base.GetAttackMessage(creature, damageType, amount);
    }

    //public override void OnDamageTarget(WorldObject target, CombatType attackType, bool critical)
    //{
    //    base.OnDamageTarget(target, attackType, critical);
    //}
}