namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Boss))]
public class Boss : CreatureEx
{
    public Boss(Biota biota) : base(biota) { }
#if REALM
    public Boss(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Boss(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        ObjScale *= 3f;
        XpOverride *= 1000;
        Name = "Tyrannical " + Name;

        var attributesToScale = Enum.GetValues<PropertyAttribute>();
        this.ScaleAttributeBase(2f, attributesToScale);
        SetMaxVitals();

        //More frequent heartbeat for lazier timing
        HeartbeatInterval = 1;
        ReinitializeHeartbeats();
    }

    public override bool VitalHeartBeat()
    {
        //Only run if healing occurred
        if (!base.VitalHeartBeat()) return false;

        //if (AttackTarget is null) return; //Require a player target
        var time = Timers.PortalYearTicks;
        UpdateWeakness(time);
        SpamCasts(time);
        DoBullethell(time);

        return true;
    }

    #region Weakness
    //Only weak to one type of damage at a time
    static readonly List<DamageType> DamageTypeValues = new() {
        DamageType.Slash,
        DamageType.Pierce,
        DamageType.Bludgeon,
        DamageType.Cold,
        DamageType.Fire,
        DamageType.Acid,
        DamageType.Electric,
        //DamageType.Physical,
        //DamageType.Elemental,
        //DamageType.Health,    
        //DamageType.Stamina,    
        //DamageType.Mana,    
        DamageType.Nether,
    };

    const int weaknessInterval = 10;
    double lastWeakness = Timers.PortalYearTicks;
    DamageType weakTo = DamageType.Nether;
    private void UpdateWeakness(double time)
    {
        //Gate time
        var lapsed = time - lastWeakness;
        if (lapsed < weaknessInterval) return;
        lastWeakness = time;

        var nextWeakness = DamageTypeValues.GetRandom();

        if (nextWeakness != weakTo)
        {
            weakTo = nextWeakness;
            DoWorldBroadcast($"{Name} is now weak to {weakTo.GetName()} after {lapsed} seconds", ChatMessageType.Broadcast);
        }
    }

    public override uint TakeDamage(WorldObject source, DamageType damageType, float amount, bool crit = false)
    {
        if (weakTo.HasFlag(damageType))
            return base.TakeDamage(source, damageType, amount, crit);

        //Reduced/nil damage from other sources
        return base.TakeDamage(source, damageType, amount / 10, crit);
    }
    #endregion

    #region Spam
    const float spamDistance = 40f;
    static readonly List<int> spellPool = new() {
        //Bolts
        63, 69, 74, 80, 74, 80, 85, 91, 97,
        //Debuffs
        183, 234, 285, 1200 ,                           
        //Attr debuffs
        1372,1396,1420,1444,1468, 
        //Vulns
        526, 1053, 1065,1089, 1132, 1156, 1242,1254, 
        //Streak
        1831,1801,1813,1807,1819,1825, 
        //Boss
        1834, 1836, 2045, 2044, 2043,2036, 2035,  
        //Arc
        2716, 2723, 2730, 2737, 2744, 2751, 2758,
};
    const int spamVariety = 3;
    const float spamDelay = 1f;
    const int spamBursts = 5;
    const int spamInterval = 7;
    double lastSpam = Timers.PortalYearTicks;
    private void SpamCasts(double time)
    {
        var lapsed = time - lastSpam;
        if (lapsed < spamInterval) return;
        lastSpam = time;

        this.PlayAnimation(PlayScript.WeddingBliss);
        DoWorldBroadcast($"{Name} is casting rapidly after {lapsed} seconds", ChatMessageType.Broadcast);

        //Queue up 5 streaks against all nearby players
        var actionChain = new ActionChain();
        for (var i = 0; i < spamBursts; i++)
        {
            actionChain.AddDelaySeconds(spamDelay);
            actionChain.AddAction(this, () =>
            {
                var spellIds = spellPool.GetRandomElements(spamVariety).ToArray();
                int count = 0;
                foreach (var nearbyPlayer in CurrentLandblock.players.Where(x => x.GetDistance(this) < spamDistance))
                    TryCastSpell(new Spell(spellIds[count++ % spellIds.Length]), nearbyPlayer);
            });
        }
        actionChain.EnqueueChain();
    }
    #endregion

    #region BulletHell
    static readonly List<int> bulletPool = new() {
        //Bolts
        63, 69, 74, 80, 74, 80, 85, 91, 97,
        //Streak
        1831,1801,1813,1807,1819,1825, 
        //Arc
        2716, 2723, 2730, 2737, 2744, 2751, 2758,
};
    const float bulletDelay = .16f;
    const int bulletCount = 50;
    const int perturbance = 10;
    const float lowVelocity = .1f;
    const float highVelocity = .5f;
    const int bulletInterval = 10;
    const int bulletHellInterval = 10;
    double lastBullHell = Timers.PortalYearTicks;
    private void DoBullethell(double time)
    {
        var lapsed = time - lastBullHell;
        if (lapsed < bulletHellInterval) return;

        if (AttackTarget is not Player target)
            return;

        lastBullHell = time;
        target.SendMessage($"{Name} is targeting you after {lapsed} seconds.  Try to dodge...");

        var actionChain = new ActionChain();
        for (var i = 0; i < bulletCount; i++)
        {
            var spell = new Spell(bulletPool.GetRandom());
            var spellType = ProjectileSpellType.Bolt;
            var origins = CalculateProjectileOrigins(spell, spellType, target);
            var velocity = CalculateProjectileVelocity(spell, target, spellType, origins[0]);

            actionChain.AddDelaySeconds(bulletDelay);
            actionChain.AddAction(this, () =>
            {
                //origins[0] += System.Numerics.Vector3.UnitZ;
                origins[0] += new Vector3(ThreadSafeRandom.Next(0, perturbance), ThreadSafeRandom.Next(0, perturbance), ThreadSafeRandom.Next(0, perturbance));
                velocity = Vector3.Multiply(CalculateProjectileVelocity(spell, target, spellType, origins[0]), (float)ThreadSafeRandom.Next(lowVelocity, highVelocity));
                LaunchSpellProjectiles(spell, target, spellType, null, false, false, origins, velocity);
            });
        }
        actionChain.AddAction(this, () =>
        {
            target?.SendMessage($"All over.");
        });
        actionChain.EnqueueChain();
    }
    #endregion

    #region Resist/Evade
    //Unresistable?
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(TryResistSpell), new Type[] { typeof(WorldObject), typeof(Spell), typeof(WorldObject), typeof(bool) })]
    public static bool PreTryResistSpell(WorldObject target, Spell spell, WorldObject itemCaster, bool projectileHit, ref WorldObject __instance, ref bool __result)
    {
        if (__instance is Boss b)
        {
            __result = false;
            return false;
        }

        return true;
    }

    //Unevadable?
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DamageEvent), nameof(DamageEvent.GetEvadeChance), new Type[] { typeof(Creature), typeof(Creature) })]
    public static bool PreGetEvadeChance(Creature attacker, Creature defender, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is Boss)
        {
            __result = 0;
            return false;
        }

        return true;
    }
    #endregion
}