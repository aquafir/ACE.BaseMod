namespace Expansion.Enums;

public enum Feature
{
    /// <summary>
    /// Augments a property of equipment on leveling based on its type
    /// </summary>
    ItemLevelUpGrowth,
    CorpseInfo,
    PetAttackSelected,
    PetMessageDamage,
    PetStow,
    ProcOnAttack,
    ProcOnHit,
    ProcRateOverride,
    //ReduceBurden,
    SummonCreatureAsPet,
    FakeXpBoost,
    /// <summary>
    /// Caches FakeInts and FakeFloats on equipment
    /// </summary>
    FakePropertyCache,
    FakeLeech,
    //FakeReducedBurden,
    FakePercentDamage,
    FakeCombo,
    FakeCulling,
    FakeItemLoot,
    FakeKillTask,
    FakeAttributes,
    FakeReflection,
    FakeSpellReflection,
    FakeDurability,
    FakeEquipRestriction,
    AutoLoot,
    FakeSpellMeta,
    FakeSpellSplitSplash,
    FakeSpellChain,
    /// <summary>
    /// Enables patches that enable Mutators to mutate WorldObjects
    /// </summary>
    MutatorHooks,
    Hardcore,
    Ironman,
    CreatureEx,
    /// <summary>
    /// Splits or splashes a missile attack
    /// </summary>
    FakeMissileSplitSplash,
    /// <summary>
    /// Override normal behavior for switching on 3 missed ranged attacks to a max ammo regardless of hitting
    /// </summary>
    CreatureMaxAmmo,
    /// <summary>
    /// Breaks up code for spell projectiles to enable custom handling
    /// </summary>
    OverrideSpellProjectiles,
    /// <summary>
    /// On logging out it stores the amount of time in game for a more accurate measure of total time played
    /// </summary>
    TimeInGame,
    DamageOverTimeConversion,
    /// <summary>
    /// Enables scaling of Life projectiles by ElementalDamageMod
    /// </summary>
    LifeMagicElementalMod,
}

