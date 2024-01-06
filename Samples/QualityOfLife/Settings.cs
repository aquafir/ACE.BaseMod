using ACE.Entity.Enum.Properties;

namespace QualityOfLife;

public class Settings
{
    // Your settings here
    public float DieSeconds { get; set; } = 0.0f;

    //Sum of specialization credits
    public int MaxSpecCredits { get; set; } = 70;

    #region Animations
    public bool OverrideAnimations { get; set; } = true;
    public Dictionary<MotionCommand, float> AnimationSpeeds { get; set; } = new()
    {
        [MotionCommand.AllegianceHometownRecall] = 0f,
        [MotionCommand.HouseRecall] = 0f,
        [MotionCommand.LifestoneRecall] = 0f,
        [MotionCommand.MarketplaceRecall] = 0f,
        [MotionCommand.PKArenaRecall] = 0f,
    };
    #endregion

    #region Properties
    public bool OverrideDefaultProperties { get; set; } = true;
    public Dictionary<PropertyInt64, long> Int64Defaults { get; set; } = new()
    {
        [PropertyInt64.MaximumLuminance] = 1_000_000_000,
    };
    public Dictionary<PropertyInt, int> IntDefaults { get; set; } = new()
    {
    };
    public Dictionary<PropertyFloat, float> FloatDefaults { get; set; } = new()
    {
    };
    public Dictionary<PropertyBool, bool> BoolDefaults { get; set; } = new()
    {
    };
    #endregion

    #region Fellow
    public bool PatchFellowships { get; set; } = true;
    public bool SendDetails { get; set; } = true;
    public int MaxMembers { get; set; } = 2;

    public Dictionary<int, double> SharePercent { get; set; } = new()
    {
        [1] = 1.0,
        [2] = .75,
        [3] = .6,
        [4] = .55,
        [5] = .5,
        [6] = .45,
        [7] = .4,
        [8] = .35,
        [9] = .3,
    };
    public double DefaultShare { get; set; } = 0;

    public long EvenShareLevel = 50;  //fellowship_even_share_level

    //Setting readonly wasn't working, bit more involved with some hardcoded kill tasks
    //public int MaxDistance { get; set; } = 600;
    #endregion
}