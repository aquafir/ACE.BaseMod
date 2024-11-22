

namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Horde))]
public class Horde : CreatureEx
{
    public Horde(Biota biota) : base(biota) { }
#if REALM
    public Horde(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Horde(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();
        baseHealth = Health.MaxValue;
        //totalCount = (uint)ThreadSafeRandom.Next(2, 10);
        SetSwarmSize((uint)ThreadSafeRandom.Next(2, 10));
    }

    uint Living => 1 + Health.Current / baseHealth;
    string OriginalName => Weenie.GetName(); //Weenie.GetPluralName();
    string CurrentName => Living switch
    {
        1 => OriginalName,
        _ => $"Swarm of {Living}/{totalCount} {OriginalName}"
    };

    uint totalCount;
    uint baseHealth;

    //When damage is taken check if a member of the swarm has been killed
    public override int UpdateVital(CreatureVital vital, int newVal)
    {

        return base.UpdateVital(vital, newVal);
    }

    public override uint TakeDamage(WorldObject source, DamageType damageType, float amount, bool crit = false)
    {
        var oldAmount = Living;
        var result = base.TakeDamage(source, damageType, amount, crit);

        var killed = oldAmount - Living;
        if (killed > 0)
        {
            if (source is Player player)
            {
                player.SendMessage($"You've killed {killed} {OriginalName}");

                for (var i = 0; i < killed; i++)
                {
                    DamageHistoryInfo info = new(player, baseHealth);
                    this.CreateCorpse(info);
                }
            }

            UpdateSwarm();
        }

        return result;
    }

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

        //Auto growth
        var autogrowth = (uint)ThreadSafeRandom.Next(0, 2);
        SetSwarmSize(autogrowth);

        //Get something to merge with of the same wcid but not a merger?
        var mergeTargets = p.GetSplashTargets(this, TargetExclusionFilter.OnlyCreature, 5).Take(5)
            .Where(x => x.WeenieClassId == WeenieClassId && x is not Horde);
        //.FirstOrDefault();

        if (mergeTargets is null || mergeTargets.Count() == 0) return;

        totalCount += (uint)mergeTargets.Count();
        foreach (var target in mergeTargets)
        {
            //Add health
            Health.Ranks += baseHealth;
            Health.Current += target.Health.Current;
            DamageHistory.OnHeal(target.Health.Current);

            //Increase xp?
            XpOverride += Weenie.GetProperty(PropertyInt.XpOverride);

            target.PlayAnimation(PlayScript.LevelUp);
            target.OnDeath();
            target.Die();
            merges++;

            count = interval;
        }

        //Scale stats
        //this.ScaleAttributeBase(1.05, Enum.GetValues<PropertyAttribute>());

        UpdateSwarm();
        p.SendMessage($"{Name} has merged {merges} times.");
    }


    void SetSwarmSize(uint count)
    {
        //Set max health / etc.
        if (count <= 0)
            return;

        totalCount += count;
        Health.Ranks += baseHealth * count;
        Health.Current += baseHealth * count;
        XpOverride += Weenie.GetProperty(PropertyInt.XpOverride) * (int)count;
        SetSwarmSize(count);
        UpdateSwarm();

        if (AttackTarget is Player player)
        {
            player.SendMessage($"{Name} has added {count} to their number...");
        }
    }


    const int scaling = 20;
    static PropertyAttribute[] props =  {
        PropertyAttribute.Coordination,
        PropertyAttribute.Endurance,
        PropertyAttribute.Focus,
        PropertyAttribute.Quickness,
        PropertyAttribute.Strength,
        PropertyAttribute.Self
    };
    void UpdateSwarm()
    {
        Name = CurrentName;

        foreach (var prop in props)
            SetAttribute(this, Living * scaling, prop);
    }
    public void SetAttribute(Creature wo, float value, PropertyAttribute prop, bool broadcast = true)
    {
        wo.Attributes[prop].Ranks = (uint)(value);
        wo.EnqueueBroadcast(new GameMessagePrivateUpdateAttribute(wo, wo.Attributes[prop]));
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.MeleeAttack))]
    public static void PostMeleeAttack(ref Creature __instance, ref float __result)
    {
        if (__instance is not Horde horde || __instance.AttackTarget is not Creature defender)
            return;

        for (var i = 0; i < horde.totalCount; i++)
        {
            if (!horde.TrySimulateMeleeDamage(out var damageEvents))
                return;

            //if (defender is Player player)
            //    player.SendMessage($"Simulated {damageEvents.FirstOrDefault().Damage}");

            horde.ActualizeDamageEvents(defender, damageEvents);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProjectileCollisionHelper), nameof(ProjectileCollisionHelper.OnCollideObject), new Type[] { typeof(WorldObject), typeof(WorldObject) })]
    public static bool PreOnCollideObject(WorldObject worldObject, WorldObject target)
    {
        //Skip redundant checks
        if (!worldObject.PhysicsObj.is_active()) return false;

        if (worldObject.ProjectileTarget == null || worldObject.ProjectileTarget != target)
        {
            ProjectileCollisionHelper.OnCollideEnvironment(worldObject);
            return false;
        }

        //Handle horde attacks
        if (worldObject.ProjectileSource is not Horde horde || target is not Creature defender)
            return true;

        for (var i = 1; i < horde.totalCount; i++)
        {
            var dmgEvent = horde.SimulateDamage(defender, worldObject);
            bool rollProc = true;
            horde.ActualizeDamageEvent(defender, dmgEvent, ref rollProc);
        }

        return true;
    }
}