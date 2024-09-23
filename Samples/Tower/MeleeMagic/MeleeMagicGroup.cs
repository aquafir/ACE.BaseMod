namespace Tower;

/// <summary>
/// MeleeMagic groups handle MeleeMagic for the weapons whose FakeDID.MeleeMagicGroup point to them
/// </summary>
public class MeleeMagicGroup
{
    /// <summary>
    /// Map attack Height to a set of pools of spells/breakpoints
    /// </summary>
    public Dictionary<AttackHeight, List<MeleeMagicPool>> Pools { get; set; } = new()
    {
        //Default group shows off 
        [AttackHeight.Low] = new() {
            new ()
            {
                MinimumSlider = .4f,
                LimitingSkill = Skill.VoidMagic,
                Spells = new ()
                {
                    [94] = SpellId.Corruption2,
                    [329] = SpellId.Corruption6,
                }
            },
        },
        [AttackHeight.Medium] = new() {
            new()
            {
                MinimumSlider = .2f,
                LimitingSkill = Skill.LifeMagic,
                Spells = new()
                {
                    [94] = SpellId.LightningArc2,
                    [235] = SpellId.LightningArc5,
                    [282] = SpellId.EyeOfTheStormII,
                }
            },
            new()
            {
                MinimumSlider = 1f,
                LimitingSkill = Skill.CreatureEnchantment,
                Spells = new()
                {
                    [0] = SpellId.VulnerabilityOther1,
                    [141] = SpellId.VulnerabilityOther3,
                    [282] = SpellId.VulnerabilityOther6,
                }
            },
        },
        [AttackHeight.High] = new() {
            new ()
            {
                MinimumSlider = .4f,
                LimitingSkill = Skill.LifeMagic,
                Spells = new ()
                {
                    [94] = SpellId.LightningVulnerabilityOther1,
                    [235] = SpellId.LightningVulnerabilityOther4,
                    [329] = SpellId.LightningVulnerabilityOther6,
                }
            },
        },
    };
}
