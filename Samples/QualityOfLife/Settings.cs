using ACE.Entity.Enum.Properties;

namespace QualityOfLife;

public class Settings
{
    // Your settings here
    public float DieSeconds { get; set; } = 0.0f;

    //Sum of specialization credits
    public int MaxSpecCredits { get; set; } = 70;

    #region Animations
    public const string AnimationOverrideCategory = "AnimationOverride";
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
    public const string DefaultOverrideCategory = "DefaultOverride";
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

}