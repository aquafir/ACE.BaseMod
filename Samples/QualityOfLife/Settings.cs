namespace QualityOfLife;

public class Settings
{
    public Patches[] Patches { get; set; } = Enum.GetValues<Patches>();


    //Sum of specialization credits
    public int MaxSpecCredits { get; set; } = 70;

    public AnimationSettings Animations { get; set; } = new();
    public DefaultsSettings Defaults { get; set; } = new();
    public FellowshipSettings Fellowship { get; set; } = new();
    public RecklessnessSettings Recklessness { get; set; } = new();
}

public enum Patches
{
    Animations,
    Defaults,
    Fellowships,
    PermanentObjects,
    Recklessness,
    SpellDefault,
}