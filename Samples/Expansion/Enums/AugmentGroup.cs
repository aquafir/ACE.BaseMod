namespace Expansion.Enums;

public enum Augment
{
    //Function
    ArmorLevel,
    ArmorModVsPierce,
    ArmorModVsSlash,
    ArmorModVsBludgeon,
    ArmorModVsAcid,
    ArmorModVsFire,
    ArmorModVsCold,
    ArmorModVsElectric,

    //Armor
    Steel,
    Alabaster,
    Bronze,
    Marble,
    ArmoredilloHide,
    Ceramic,
    Wool,
    ReedSharkHide,
    Peridot,
    YellowTopaz,
    Zircon,
    //Item
    Pine,
    Gold,
    Linen,
    Leather,
    Moonstone,
    Sandstone,
    Copper,
    Silver,
    Silk,
    //Magic Item
    Sunstone,
    FireOpal,
    BlackOpal,
    Opal,
    GreenGarnet,
    //Weapon
    Iron,
    Mahogany,
    Granite,
    Oak,
    Brass,
    Velvet,
    Emerald,
    WhiteSapphire,
    Aquamarine,
    Jet,
    RedGarnet,
    BlackGarnet,
    ImperialTopaz,
    FetishOfTheDarkIdols,
    //Custom
    RendAll,
    Big,
}

public enum AugmentGroup
{
    Full,
    Armor,
    Item,
    MagicItem,
    Weapon,
    //Rending,
    //WeaponImbue,
    //ArmorImbue,
}

public static class AugmentHelper
{
    public static Augment[] SetOf(this AugmentGroup type) => type switch
    {
        AugmentGroup.Full => Enum.GetValues<Augment>(),
        AugmentGroup.Armor => new[]
        {
            Augment.Steel,
            Augment.Alabaster,
            Augment.Bronze,
            Augment.Marble,
            Augment.ArmoredilloHide,
            Augment.Ceramic,
            Augment.Wool,
            Augment.ReedSharkHide,
            Augment.Peridot,
            Augment.YellowTopaz,
            Augment.Zircon,
        },
        AugmentGroup.Item => new[]
        {
            Augment.Pine,
            Augment.Gold,
            Augment.Linen,
            Augment.Leather,
            Augment.Moonstone,
            Augment.Sandstone,
            Augment.Copper,
            Augment.Silver,
            Augment.Silk,
        },
        AugmentGroup.MagicItem => new[]
        {
            Augment.Sunstone,
            Augment.FireOpal,
            Augment.BlackOpal,
            Augment.Opal,
            Augment.GreenGarnet,
        },
        AugmentGroup.Weapon => new[]
        {
            Augment.Iron,
            Augment.Mahogany,
            Augment.Granite,
            Augment.Oak,
            Augment.Brass,
            Augment.Velvet,
            Augment.Emerald,
            Augment.WhiteSapphire,
            Augment.Aquamarine,
            Augment.Jet,
            Augment.RedGarnet,
            Augment.BlackGarnet,
            Augment.ImperialTopaz,
            Augment.FetishOfTheDarkIdols
        },
        //AugmentGroup.Rending => new[]
        //{
        //    Augment.Emerald,
        //    Augment.WhiteSapphire,
        //    Augment.Aquamarine,
        //    Augment.Jet,
        //    Augment.RedGarnet,
        //    Augment.BlackGarnet,
        //    Augment.ImperialTopaz,
        //},
        _ => throw new NotImplementedException(), //Array.Empty<Augment>(), //Todo: decide on exception or 
    };

    /// <summary>
    /// Attempts to apply an augment to a WorldObject
    /// </summary>
    public static bool TryAugmentWith(this WorldObject target, Augment augment)
    {
        //Starting from RecipeManager.TryMutateNative
        switch (augment)
        {
            #region armor tinkering
            case Augment.Steel:
                target.ArmorLevel += 20;
                return true;
            // mutations apparently didn't cap to 2.0 here, clamps are applied in damage calculations though
            case Augment.Alabaster:
                target.ArmorModVsPierce += 0.2f;
                return true;
            case Augment.Bronze:
                target.ArmorModVsSlash += 0.2f;
                return true;
            case Augment.Marble:
                target.ArmorModVsBludgeon += 0.2f;
                return true;
            case Augment.ArmoredilloHide:
                target.ArmorModVsAcid += 0.4f;
                return true;
            case Augment.Ceramic:
                target.ArmorModVsFire += 0.4f;
                return true;
            case Augment.Wool:
                target.ArmorModVsCold += 0.4f;
                return true;
            case Augment.ReedSharkHide:
                target.ArmorModVsElectric += 0.4f;
                return true;
            case Augment.Peridot:
                target.ImbuedEffect = ImbuedEffectType.MeleeDefense;
                return true;
            case Augment.YellowTopaz:
                target.ImbuedEffect = ImbuedEffectType.MissileDefense;
                return true;
            case Augment.Zircon:
                target.ImbuedEffect = ImbuedEffectType.MagicDefense;
                return true;
            #endregion

            #region item tinkering
            case Augment.Pine:
                target.Value = (int?)(target.Value * 0.75f);
                return true;
            case Augment.Gold:
                target.Value = (int?)(target.Value * 1.25f);
                return true;
            case Augment.Linen:
                target.EncumbranceVal = (int?)(target.EncumbranceVal * 0.75f);
                return true;
            // Ivory is handled purely in recipe mod?
            case Augment.Leather:
                target.Retained = true;
                return true;
            case Augment.Sandstone:
                target.Retained = false;
                return true;
            case Augment.Moonstone:
                target.ItemMaxMana += 500;
                return true;
            case Augment.Copper:
                // handled in requirements, only here for legacy support?
                if (target.ItemSkillLimit != Skill.MissileDefense || target.ItemSkillLevelLimit == null)
                    return false;
                // change activation requirement: missile defense -> melee defense
                target.ItemSkillLimit = Skill.MeleeDefense;
                target.ItemSkillLevelLimit = (int)(target.ItemSkillLevelLimit / 0.7f);
                return true;
            case Augment.Silver:
                // handled in requirements, only here for legacy support?
                if (target.ItemSkillLimit != Skill.MeleeDefense || target.ItemSkillLevelLimit == null)
                    return false;
                // change activation requirement: melee defense -> missile defense
                target.ItemSkillLimit = Skill.MissileDefense;
                target.ItemSkillLevelLimit = (int)(target.ItemSkillLevelLimit * 0.7f);
                return true;
            case Augment.Silk:
                // remove allegiance rank limit, set difficulty to spellcraft
                target.ItemAllegianceRankLimit = null;
                target.ItemDifficulty = target.ItemSpellcraft;
                return true;
            // armatures / trinkets
            // these are handled in recipe mod
            //case Augment.Amber:
            //case Augment.Diamond:
            //case Augment.GromnieHide:
            //case Augment.Pyreal:
            //case Augment.Ruby:
            //case Augment.Sapphire:
            //    return false; 
            #endregion

            #region magic item tinkering
            case Augment.Sunstone:
                //AddImbuedEffect(target, ImbuedEffectType.ArmorRending);
                target.ImbuedEffect = ImbuedEffectType.ArmorRending;
                return true;
            case Augment.FireOpal:
                //AddImbuedEffect(target, ImbuedEffectType.CripplingBlow);
                target.ImbuedEffect = ImbuedEffectType.CripplingBlow;
                return true;
            case Augment.BlackOpal:
                //AddImbuedEffect(target, ImbuedEffectType.CriticalStrike);
                target.ImbuedEffect = ImbuedEffectType.CriticalStrike;
                return true;
            case Augment.Opal:
                //target.ManaConversionMod += 0.01f;
                target.ManaConversionMod = (target.ManaConversionMod ?? 0.0f) + 0.01f;
                return true;
            case Augment.GreenGarnet:
                target.ElementalDamageMod = (target.ElementalDamageMod ?? 0.0f) + 0.01f;     // + 1% vs. monsters, + 0.25% vs. players
                return true;
            //            case 0x38000041:
            // these are handled in recipe mods already
            // SmokeyQuartz
            //AddSpell(player, target, SpellId.CANTRIPCOORDINATION1);
            // RoseQuartz
            //AddSpell(player, target, SpellId.CANTRIPQUICKNESS1);
            // RedJade
            //AddSpell(player, target, SpellId.CANTRIPHEALTHGAIN1);
            // Malachite
            //AddSpell(player, target, SpellId.WarriorsVigor);
            // LavenderJade
            //AddSpell(player, target, SpellId.CANTRIPMANAGAIN1);
            // LapisLazuli
            //AddSpell(player, target, SpellId.CANTRIPWILLPOWER1);
            // Hematite
            //AddSpell(player, target, SpellId.WarriorsVitality);
            // Citrine
            //AddSpell(player, target, SpellId.CANTRIPSTAMINAGAIN1);
            // Carnelian
            //AddSpell(player, target, SpellId.CANTRIPSTRENGTH1);
            // Bloodstone
            //AddSpell(player, target, SpellId.CANTRIPENDURANCE1);
            // Azurite
            //AddSpell(player, target, SpellId.WizardsIntellect);
            // Agate
            //AddSpell(player, target, SpellId.CANTRIPFOCUS1);
            //      target.ImbuedEffect = ImbuedEffectType.Spellbook;
            //     return true; 
            #endregion

            #region weapon tinkering
            case Augment.Iron:
                target.Damage += 1;
                return true;
            case Augment.Mahogany:
                target.DamageMod += 0.04f;
                return true;
            case Augment.Granite:
                target.DamageVariance *= 0.8f;
                return true;
            case Augment.Oak:
                target.WeaponTime = Math.Max(0, (target.WeaponTime ?? 0) - 50);
                return true;
            case Augment.Brass:
                target.WeaponDefense += 0.01f;
                return true;
            case Augment.Velvet:
                target.WeaponOffense += 0.01f;
                return true;
            case Augment.Emerald:
                target.ImbuedEffect = ImbuedEffectType.AcidRending;
                return true;
            case Augment.WhiteSapphire:
                target.ImbuedEffect = ImbuedEffectType.BludgeonRending;
                return true;
            case Augment.Aquamarine:
                target.ImbuedEffect = ImbuedEffectType.ColdRending;
                return true;
            case Augment.Jet:
                target.ImbuedEffect = ImbuedEffectType.ElectricRending;
                return true;
            case Augment.RedGarnet:
                target.ImbuedEffect = ImbuedEffectType.FireRending;
                return true;
            case Augment.BlackGarnet:
                target.ImbuedEffect = ImbuedEffectType.PierceRending;
                return true;
            case Augment.ImperialTopaz:
                target.ImbuedEffect = ImbuedEffectType.SlashRending;
                return true;
            case Augment.FetishOfTheDarkIdols:
                if (target.ImbuedEffect >= ImbuedEffectType.IgnoreAllArmor)
                    target.ImbuedEffect = ImbuedEffectType.Undef;
                target.ImbuedEffect |= ImbuedEffectType.IgnoreSomeMagicProjectileDamage;
                //target.AbsorbMagicDamage = 0.25f;   // not in original mods / mutation?
                return true;
            case Augment.RendAll:
                target.ImbuedEffect |= ImbuedEffectType.AcidRending | ImbuedEffectType.ColdRending | ImbuedEffectType.FireRending | ImbuedEffectType.ElectricRending |
                    ImbuedEffectType.BludgeonRending | ImbuedEffectType.PierceRending | ImbuedEffectType.SlashRending | ImbuedEffectType.NetherRending;
                target.IconUnderlayId ??= IconUnderlay[ImbuedEffectType.Undef];
                return true;

            case Augment.Big:
                //Debugger.Break();
                if (target.ObjScale is null) return false;
                target.ObjScale *= 1.2f;    //Cap, flat, etc. possible

                if (PlayerManager.GetOnlinePlayer(target.OwnerId ?? 0) is Player player)
                    player.UpdateProperty(target, PropertyFloat.DefaultScale, target.ObjScale, true);


                return true;
            default:
                return false;
                #endregion
        }
        return false;
    }

    private static readonly Dictionary<ImbuedEffectType, uint> IconUnderlay = new Dictionary<ImbuedEffectType, uint>()
        {
            { ImbuedEffectType.ColdRending,     0x06003353 },
            { ImbuedEffectType.ElectricRending, 0x06003354 },
            { ImbuedEffectType.AcidRending,     0x06003355 },
            { ImbuedEffectType.ArmorRending,    0x06003356 },
            { ImbuedEffectType.CripplingBlow,   0x06003357 },
            { ImbuedEffectType.CriticalStrike,  0x06003358 },
            { ImbuedEffectType.FireRending,     0x06003359 },
            { ImbuedEffectType.BludgeonRending, 0x0600335a },
            { ImbuedEffectType.PierceRending,   0x0600335b },
            { ImbuedEffectType.SlashRending,    0x0600335c },
            {ImbuedEffectType.Undef ,           0x6005B0C },
        };
}
