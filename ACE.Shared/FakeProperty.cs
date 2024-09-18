namespace ACE.Shared;

//!!!!Very important to avoid using properties ACE or other Mods uses!!!!
public enum FakeBool
{
    GrowthItem = 10000,
    CorpseSpawnedDungeon = 10001,
    Ironman = 10002,
    Hardcore = 10003,
    ShadowBanned = 10004,
    UsingNewLuminance = 10005,
    /// <summary>
    /// Permanent objects will not be removed when a landblock respawns
    /// </summary>
    Permanent = 10006,
    BankUsesDirectDeposit = 10007,
    Quarantined = 10008,
    MeleeMagic = 10009,
}
public enum FakeDID
{
    CorpseLandblockId = 10000,
    LocationLockId = 10001,
    MeleeMagicGroup = 10002
}
public enum FakeFloat
{
    ItemLeechHealth = 10001,
    ItemLeechMana = 10002,
    ItemLeechStamina = 10003,
    ItemPercentMaxHealthDamage = 10004,
    ItemPercentCurrentHealthDamage = 10005,
    ItemPercentMissingHealthDamage = 10006,
    TimestampLastKill = 10007,
    TimestampLastHit = 10008,
    TimestampLastEvade = 10009,
    TimestampLastCast = 10010,
    TimestampLastResist = 10011,
    ItemPercentCull = 10012,
    ItemLootTierUpgrade = 10013,
    LootItemMaxAmount = 10014,
    LootItemQualityMod = 10015,
    ItemReflectPercent = 10016,
    ItemReflectSpellProjectileChance = 10017,
    ItemReflectSpellChance = 10018,
    ItemXpBoost = 10019,
    ItemLuminanceBoost = 10020,
    TimestampLastSpellSplash = 10021,
    TimestampLastSpellSplit = 10022,
    ItemSpellSplitRangeScale = 10023,
    ItemSpellSplitCooldownScale = 10024,
    ItemSpellSplashRangeScale = 10025,
    ItemSpellSplashCooldownScale = 10026,
    ItemSpellMetaPower = 10027,
    SpellChainChance = 10028,
    /// <summary>
    /// Used by PetSummonMultiple to define non-default weights for a Pet that count against the total
    /// </summary>
    PetWeight = 10029,
    PetWeightMax = 10030,



    //Goal value
    ItemWieldRequirementValue1 = 20000,
    TimestampLastPlayerDeath = 20001,
    QuestBonus = 20002,
    TimestampLastMissileSplit = 20003,
    ItemMissileSplitAngle = 20004,
    TimestampLastMissileSplash = 20005,
    ItemMissileSplashRadius = 20006,
    /// <summary>
    /// Time previously spent in game, used in conjunction with LoginTimestamp to find current time in game
    /// </summary>
    TimeInGame = 20007,
    ItemMissileSplashCooldown = 20008,
    ItemMissileSplitCooldown = 20009,
}
public enum FakeIID
{
    PetDevice = 10000
}
public enum FakeInt
{
    CorpseLivingWCID = 10000,
    GrowthTier = 10001,
    ItemPercentMaxHealthCap = 10002,
    ItemPercentCurrentHealthCap = 10003,
    ItemPercentMissingHealthCap = 10004,
    ComboKill = 10005,
    ComboHit = 10006,
    ComboEvade = 10007,
    ComboResist = 10008,
    ItemKillTaskBonus = 10009,
    ItemStrengthMod = 10010,
    ItemEnduranceMod = 10011,
    ItemQuicknessMod = 10012,
    ItemCoordinationMod = 10013,
    ItemFocusMod = 10014,
    ItemSelfMod = 10015,
    LootMundaneItemChance = 10016,
    LootMundaneItemMinAmount = 10017,
    LootMundaneItemMaxAmount = 10018,
    LootItemChance = 10019,
    LootItemMinAmount = 10020,
    LootItemMaxAmount = 10021,
    LootMagicItemChance = 10022,
    LootMagicItemMinAmount = 10023,
    LootMagicItemMaxAmount = 10024,
    ItemReflectFlat = 10025,
    OriginalItemType = 10026,
    ItemSpellSplitCount = 10027,
    ItemSpellSplashCount = 10028,
    CreatureExType = 10029,

    /// <summary>
    /// Flat bonus to skills
    /// </summary>
    BonusAxe,
    BonusBow,
    BonusCrossbow,
    BonusDagger,
    BonusMace,
    BonusMeleeDefense,
    BonusMissileDefense,
    BonusSling,
    BonusSpear,
    BonusStaff,
    BonusSword,
    BonusThrownWeapon,
    BonusUnarmedCombat,
    BonusArcaneLore,
    BonusMagicDefense,
    BonusManaConversion,
    BonusSpellcraft,
    BonusItemTinkering,
    BonusAssessPerson,
    BonusDeception,
    BonusHealing,
    BonusJump,
    BonusLockpick,
    BonusRun,
    BonusAwareness,
    BonusArmsAndArmorRepair,
    BonusAssessCreature,
    BonusWeaponTinkering,
    BonusArmorTinkering,
    BonusMagicItemTinkering,
    BonusCreatureEnchantment,
    BonusItemEnchantment,
    BonusLifeMagic,
    BonusWarMagic,
    BonusLeadership,
    BonusLoyalty,
    BonusFletching,
    BonusAlchemy,
    BonusCooking,
    BonusSalvaging,
    BonusTwoHandedCombat,
    BonusGearcraft,
    BonusVoidMagic,
    BonusHeavyWeapons,
    BonusLightWeapons,
    BonusFinesseWeapons,
    BonusMissileWeapons,
    BonusShield,
    BonusDualWield,
    BonusRecklessness,
    BonusSneakAttack,
    BonusDirtyFighting,
    BonusChallenge,
    BonusSummoning,
    /// <summary>
    /// Flat bonus to attributes
    /// </summary>
    BonusStrength,
    BonusEndurance,
    BonusQuickness,
    BonusCoordination,
    BonusFocus,
    BonusSelf,
    /// <summary>
    /// Flat bonus to attributes
    /// </summary>
    BonusMaxHealth,
    BonusMaxStamina,
    BonusMaxMana,

    //PropertyEnum?
    ItemWieldRequirementType1 = 20000,
    //Compare
    ItemWieldRequirementKey1 = 20001,
    //Comparer
    ItemWieldRequirementCompareType1 = 20002,
    HardcoreLives = 20003,
    ChatInfractions = 20004,
    ItemWieldRequirementEnlightenments = 20005,
    DifficultyTier = 20006,
    MaxAmmo = 20007,
    ItemMissileSplitCount = 20008,
    ItemMissileSplashCount = 20009,
    SpellChainCount = 20010,
}

public enum FakeInt64
{
    Pyreals = 10000
}
public enum FakeString
{
    Placeholder = 10000,
    IronmanPlan = 20000,
    IronmanFullPlan = 20001,
}

public static class FakePropertyHelper
{
    //public static void SetProperty(this WorldObject wo, FakePropertyInt64 property, long value) => wo.SetProperty(property.Prop(), value);
    public static void SetProperty(this WorldObject wo, FakeBool property, bool value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeBool property) => wo.RemoveProperty(property.Prop());
    public static bool? GetProperty(this WorldObject wo, FakeBool property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeInt property, int value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeInt property) => wo.RemoveProperty(property.Prop());
    public static int? GetProperty(this WorldObject wo, FakeInt property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeInt64 property, long value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeInt64 property) => wo.RemoveProperty(property.Prop());
    public static long? GetProperty(this WorldObject wo, FakeInt64 property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeFloat property, double value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeFloat property) => wo.RemoveProperty(property.Prop());
    public static double? GetProperty(this WorldObject wo, FakeFloat property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeString property, string value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeString property) => wo.RemoveProperty(property.Prop());
    public static string? GetProperty(this WorldObject wo, FakeString property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeDID property, uint value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeDID property) => wo.RemoveProperty(property.Prop());
    public static uint? GetProperty(this WorldObject wo, FakeDID property) => wo.GetProperty(property.Prop());
#if REALM
    public static void SetProperty(this WorldObject wo, FakeIID property, ulong value) => wo.SetProperty(property.Prop(), value);
#else
    public static void SetProperty(this WorldObject wo, FakeIID property, uint value) => wo.SetProperty(property.Prop(), value);
#endif
    public static void RemoveProperty(this WorldObject wo, FakeIID property) => wo.RemoveProperty(property.Prop());
#if REALM
    public static ulong? GetProperty(this WorldObject wo, FakeIID property) => wo.GetProperty(property.Prop());
#else
    public static uint? GetProperty(this WorldObject wo, FakeIID property) => wo.GetProperty(property.Prop());
#endif    
    public static PropertyBool Prop(this FakeBool prop) => (PropertyBool)prop;
    public static PropertyDataId Prop(this FakeDID prop) => (PropertyDataId)prop;
    public static PropertyFloat Prop(this FakeFloat prop) => (PropertyFloat)prop;
    public static PropertyInstanceId Prop(this FakeIID prop) => (PropertyInstanceId)prop;
    public static PropertyInt Prop(this FakeInt prop) => (PropertyInt)prop;
    public static PropertyInt64 Prop(this FakeInt64 prop) => (PropertyInt64)prop;
    public static PropertyString Prop(this FakeString prop) => (PropertyString)prop;
}


public enum CombinationStyle
{
    Additive,
    Multiplicative,
}