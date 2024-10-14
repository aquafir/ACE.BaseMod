namespace Tower;

public class MeleeMagicSettings
{
    /// <summary>
    /// The number of slider divisions determines how Height and the Slider map to a group
    /// </summary>
    //public int SliderDivions { get; set; } = 2;

    public bool EnabledForPvP { get; set; } = false;

    public bool RequireDamage { get; set; } = true;

    //Todo, rethink
    /// <summary>
    /// The default group is used when unarmed or the weapon lacks a valid FakeDID.MeleeMagicGroup
    /// </summary>
    public uint DefaultGroup { get; set; } = 0;

    /// <summary>
    /// Groups 
    /// </summary>
    public Dictionary<uint, MeleeMagicGroup> MeleeMagicGroups { get; set; } = new()
    {
        [0] = new(),
        //Non-default
        [1] = new()
        {
            Pools = new()
            {
                [AttackHeight.Low] = new() {
                    new ()
                    {
                        MinimumSlider = 1,
                        LimitingSkill = Skill.VoidMagic,
                        Spells = new ()
                        {
                            [94] = SpellId.Corrosion3,
                            [329] = SpellId.Corrosion6,
                        }
                    },
                    new ()
                    {
                        MinimumSlider = .5f,
                        LimitingSkill = Skill.VoidMagic,
                        Spells = new ()
                        {
                            [94] = SpellId.FesterOther3,
                            [329] = SpellId.FesterOther6,
                        }
                    },
                },
            }
        },
    };
}

//public record struct MeleeMagicBucket(AttackHeight Height, float MinSlider);
//public record struct SkillSpellPair(int Base, SpellId Spell);