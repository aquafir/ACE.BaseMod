namespace ACE.Shared.Helpers;

public static class CreatureExtensions
{
    /// <summary>
    /// Percentage of current health
    /// </summary>
    public static float PercentHealth(this Creature creature) => (float)creature.Health.Current / creature.Health.MaxValue;


    /// <summary>
    /// Damage without source
    /// </summary>
    public static bool TryDamageDirect(this Creature creature, float amount, out uint damageTaken, DamageType damageType = DamageType.Undef, bool ignoreResistance = false)
    {
        damageTaken = 0;
        if (creature.IsDead || creature.Invincible) return false;

        // handle lifestone protection?
        if (creature is Player p && p.UnderLifestoneProtection)
        {
            p.HandleLifestoneProtection();
            return false;
        }

        // vital
        CreatureVital vital = damageType switch
        {
            DamageType.Stamina => creature.Stamina,
            DamageType.Mana => creature.Mana,
            _ => creature.Health
        };

        // scale by resistance?
        var resistance = ignoreResistance ? 1f : creature.GetResistanceMod(damageType, null, null);
        var damage = (uint)Math.Round(amount * resistance);

        // update vital
        damageTaken = (uint)-creature.UpdateVitalDelta(vital, (int)-damage);
        creature.DamageHistory.Add(creature, damageType, damageTaken);

        if (creature.Health.Current <= 0)
        {
            creature.OnDeath(new DamageHistoryInfo(creature), damageType, false);
            creature.Die();
        }

        return true;
    }

    public static void ScaleProperty(this WorldObject wo, PropertyInt property, float amount) => wo.SetProperty(property, (int)(amount * wo.GetProperty(property) ?? 0));
    public static void ScaleProperty(this WorldObject wo, PropertyFloat property, float amount) => wo.SetProperty(property, (double)(amount * wo.GetProperty(property) ?? 0));
    public static void ScaleProperty(this WorldObject wo, PropertyInt64 property, float amount) => wo.SetProperty(property, (long)(amount * wo.GetProperty(property) ?? 0));

    public static void ScaleAttributeBase(this Creature wo, float amount, params PropertyAttribute[] properties) =>
        Array.ForEach<PropertyAttribute>(properties, (property) =>
        {
            if (property != PropertyAttribute.Undef)
                wo.Attributes[property].StartingValue = (uint)(wo.Attributes[property].StartingValue * amount);
        });
    public static void ScaleAttributeBase(this Creature wo, float amount, params PropertyAttribute2nd[] properties) =>
        Array.ForEach<PropertyAttribute2nd>(properties, (property) =>
        {
            if (property != PropertyAttribute2nd.Undef)
                wo.Vitals[property].StartingValue = (uint)(wo.Vitals[property].StartingValue * amount);
        });

    public static void ScaleVital(this Creature wo, float value, PropertyAttribute2nd prop, bool broadcast = true)
    {
        wo.Vitals[prop].StartingValue = (uint)(wo.Vitals[prop].StartingValue * value);
    }
    public static void SetVital(this Creature wo, float value, PropertyAttribute2nd prop, bool broadcast = true)
    {
        wo.Vitals[prop].StartingValue = (uint)value;

        //if(broadcast)
        //todo
    }

}
