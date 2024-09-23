namespace Tower;

/// <summary>
/// MeleeMagic pools contain a Power/Accuracy slider minimal requirement and a skill used for the breakpoints of a list of spells
/// </summary>
public class MeleeMagicPool
{
    public float MinimumSlider { get; set; } = 0;
    public Skill LimitingSkill { get; set; }

    public SortedList<uint, SpellId> Spells { get; set; } = new();
}
