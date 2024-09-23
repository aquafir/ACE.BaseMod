using ACE.DatLoader.Entity;

namespace CustomSpells;
public static class SpellExtensions
{
    /// <summary>
    /// Returns a copy of SpellBase
    /// </summary>
    public static SpellBase Clone(this SpellBase spellBase)
    {
        if (spellBase is null)
            return null;

        //Would like to be able to do this in ACE.Shared but I don't want to force a publicizer dependency and the setters are private
        var clone = new SpellBase()
        {
            BaseMana = spellBase.BaseMana,
            BaseRangeConstant = spellBase.BaseRangeConstant,
            BaseRangeMod = spellBase.BaseRangeMod,
            Bitfield = spellBase.Bitfield,
            CasterEffect = spellBase.CasterEffect,
            Category = spellBase.Category,
            ComponentLoss = spellBase.ComponentLoss,
            Desc = spellBase.Desc,
            DegradeLimit = spellBase.DegradeLimit,
            DegradeModifier = spellBase.DegradeModifier,
            DisplayOrder = spellBase.DisplayOrder,
            Duration = spellBase.Duration,
            FizzleEffect = spellBase.FizzleEffect,
            Formula = spellBase.Formula.ToList(),
            FormulaVersion = spellBase.FormulaVersion,
            Icon = spellBase.Icon,
            ManaMod = spellBase.ManaMod,
            MetaSpellId = spellBase.MetaSpellId,
            MetaSpellType = spellBase.MetaSpellType,
            Name = spellBase.Name,
            NonComponentTargetType = spellBase.NonComponentTargetType,
            PortalLifetime = spellBase.PortalLifetime,
            Power = spellBase.Power,
            RecoveryAmount = spellBase.RecoveryAmount,
            RecoveryInterval = spellBase.RecoveryInterval,
            School = spellBase.School,
            SpellEconomyMod = spellBase.SpellEconomyMod,
            TargetEffect = spellBase.TargetEffect,
            spellWords = spellBase.spellWords,
        };

        return clone;
    }
}
