namespace ACE.Shared.Props;
public static class PropertyIntExtensions
{
    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    //public static string Friendly(this WorldObject wo, PropertyInt key) => wo.TryGet(key, out var value) ? key.Friendly(value) : null;
    //public static bool TryGetFriendly(this WorldObject wo, PropertyInt key, out string friendly) => wo.TryGet(key, out var value) ?
    //    (friendly = key.Friendly(value)) is not null :
    //    (friendly = null) is not null;

    ///// <summary>
    ///// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    ///// </summary>
    //public static string Friendly(this PropertyInt key, int value) => key switch
    //{
    //    PropertyInt.GeneratorEndTime => DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToString(CultureInfo.InvariantCulture),
    //    PropertyInt.GeneratorStartTime => DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToString(CultureInfo.InvariantCulture),
    //    //Default to trying to look up an enum name
    //    _ => _enums.TryGetValue(key, out var type) ? type.GetEnumName(value) ?? value.ToString() : null,
    //};
    //public static bool TryGetFriendly(this PropertyInt key, int value, out string friendly) => (friendly = key.Friendly(value)) is not null;

    ///// <summary>
    ///// Tries to find the Enum associated with a property key
    ///// </summary>
    //public static bool TryGetEnum(this PropertyInt key, out Type enumType) => _enums.TryGetValue(key, out enumType);

    ///// <summary>
    ///// Returns a descriptive label for a property, defaulting to the name of the property
    ///// </summary>
    //public static string Label(this PropertyInt key) => _labels.TryGetValue(key, out var label) ? label : key.ToString();

    ///// <summary>
    ///// Returns a formatted version of the WorldObject's property value if a format string exists, the value if it does not, and an empty string if the value is missing.
    ///// </summary>
    ////public static string Format(this WorldObject wo, PropertyInt prop)
    ////{
    ////    //Return if value missing
    ////    if (!wo.TryGet(prop, out var value))
    ////        return null;  //String.Empty no more efficient

    ////    return prop.Format(value);
    ////}
    ///// <summary>
    ///// Returns a formatted version of a property value
    ///// </summary>
    //public static string Format(this PropertyInt prop, params object[] values)
    //{
    //    //Prefer friendly name if available?
    //    if (prop.TryGetFriendly((int)values[0], out var friendly))
    //        values[0] = friendly;

    //    //Use a format string if it exists?
    //    if (_formatStrings.TryGetValue(prop, out var format))
    //        return String.Format(format, values);

    //    return values[0].ToString();
    //}

    static readonly Dictionary<PropertyInt, string> _formatStrings = new()
    {

    };

    static readonly Dictionary<PropertyInt, string> _labels = new()
    {

    };

    static readonly Dictionary<PropertyInt, Type> _enums = new()
    {
        [PropertyInt.AccountRequirements] = typeof(SubscriptionStatus),
        [PropertyInt.ActivationResponse] = typeof(ActivationResponse),
        [PropertyInt.AetheriaBitfield] = typeof(AetheriaBitfield),
        [PropertyInt.AiAllowedCombatStyle] = typeof(CombatStyle),
        [PropertyInt.AmmoType] = typeof(AmmoType),
        [PropertyInt.AppraisalItemSkill] = typeof(Skill),
        [PropertyInt.ArmorType] = typeof(ArmorType),
        [PropertyInt.AttackHeight] = typeof(AttackHeight),
        [PropertyInt.AttackType] = typeof(AttackType),
        [PropertyInt.Attuned] = typeof(AttunedStatus),
        [PropertyInt.Bonded] = typeof(BondedStatus),
        [PropertyInt.BoosterEnum] = typeof(PropertyAttribute2nd),
        [PropertyInt.ChannelsActive] = typeof(Channel),
        [PropertyInt.ChannelsAllowed] = typeof(Channel),
        [PropertyInt.ClothingPriority] = typeof(CoverageMask),
        [PropertyInt.CombatMode] = typeof(CombatMode),
        [PropertyInt.CombatUse] = typeof(CombatUse),
        [PropertyInt.CreatureType] = typeof(CreatureType),
        [PropertyInt.CurrentWieldedLocation] = typeof(EquipMask),
        [PropertyInt.DamageType] = typeof(DamageType),
        [PropertyInt.DefaultCombatStyle] = typeof(CombatStyle),
        [PropertyInt.EquipmentSetId] = typeof(EquipmentSet),
        [PropertyInt.Faction1Bits] = typeof(FactionBits),
        [PropertyInt.Faction2Bits] = typeof(FactionBits),
        [PropertyInt.Faction3Bits] = typeof(FactionBits),
        [PropertyInt.FoeType] = typeof(CreatureType),
        [PropertyInt.FriendType] = typeof(CreatureType),
        [PropertyInt.Gender] = typeof(Gender),
        [PropertyInt.GeneratorDestructionType] = typeof(GeneratorDestruct),
        [PropertyInt.GeneratorEndDestructionType] = typeof(GeneratorDestruct),
        [PropertyInt.GeneratorTimeType] = typeof(GeneratorTimeType),
        [PropertyInt.GeneratorType] = typeof(GeneratorType),
        [PropertyInt.Hatred1Bits] = typeof(FactionBits),
        [PropertyInt.Hatred2Bits] = typeof(FactionBits),
        [PropertyInt.Hatred3Bits] = typeof(FactionBits),
        [PropertyInt.HeritageGroup] = typeof(HeritageGroup),
        [PropertyInt.HeritageSpecificArmor] = typeof(HeritageGroup),
        [PropertyInt.HookGroup] = typeof(HookGroupType),
        [PropertyInt.HookItemType] = typeof(ItemType),
        [PropertyInt.HookPlacement] = typeof(Placement),
        [PropertyInt.HookType] = typeof(HookType),
        [PropertyInt.HouseStatus] = typeof(HouseStatus),
        [PropertyInt.HouseType] = typeof(HouseType),
        [PropertyInt.ImbuedEffect] = typeof(ImbuedEffectType),
        [PropertyInt.ImbuedEffect2] = typeof(ImbuedEffectType),
        [PropertyInt.ImbuedEffect3] = typeof(ImbuedEffectType),
        [PropertyInt.ImbuedEffect4] = typeof(ImbuedEffectType),
        [PropertyInt.ImbuedEffect5] = typeof(ImbuedEffectType),
        [PropertyInt.ItemType] = typeof(ItemType),
        [PropertyInt.ItemUseable] = typeof(Usable),
        [PropertyInt.ItemXpStyle] = typeof(ItemXpStyle),
        [PropertyInt.MaterialType] = typeof(MaterialType),
        [PropertyInt.MerchandiseItemTypes] = typeof(ItemType),
        [PropertyInt.PaletteTemplate] = typeof(PaletteTemplate),
        [PropertyInt.ParentLocation] = typeof(ParentLocation),
        [PropertyInt.PCAPRecordedPlacement] = typeof(Placement),
        [PropertyInt.PhysicsState] = typeof(PhysicsState),
        [PropertyInt.Placement] = typeof(Placement),
        [PropertyInt.PlacementPosition] = typeof(Placement),
        [PropertyInt.PlayerKillerStatus] = typeof(PlayerKillerStatus),
        [PropertyInt.PortalBitmask] = typeof(PortalBitmask),
        [PropertyInt.RadarBlipColor] = typeof(RadarColor),
        [PropertyInt.ResistanceModifierType] = typeof(DamageType),
        [PropertyInt.ShowableOnRadar] = typeof(RadarBehavior),
        [PropertyInt.SkillToBeAltered] = typeof(Skill),
        [PropertyInt.SlayerCreatureType] = typeof(CreatureType),
        [PropertyInt.SummoningMastery] = typeof(SummoningMastery),
        [PropertyInt.TargetType] = typeof(ItemType),
        [PropertyInt.UiEffects] = typeof(UiEffects),
        [PropertyInt.UseCreatesContractId] = typeof(ContractId),
        [PropertyInt.UseRequiresSkill] = typeof(Skill),
        [PropertyInt.UseRequiresSkillSpec] = typeof(Skill),
        [PropertyInt.ValidLocations] = typeof(EquipMask),
        [PropertyInt.WeaponSkill] = typeof(Skill),
        [PropertyInt.WeaponType] = typeof(WeaponType),
        [PropertyInt.WieldRequirements] = typeof(WieldRequirement),
        [PropertyInt.WieldRequirements2] = typeof(WieldRequirement),
        [PropertyInt.WieldRequirements3] = typeof(WieldRequirement),
        [PropertyInt.WieldRequirements4] = typeof(WieldRequirement),
        [PropertyInt.WieldSkillType] = typeof(Skill),
        [PropertyInt.WieldSkillType2] = typeof(Skill),
        [PropertyInt.WieldSkillType3] = typeof(Skill),
        [PropertyInt.WieldSkillType4] = typeof(Skill),
    };
}
