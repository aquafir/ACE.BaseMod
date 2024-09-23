namespace EasyEnlightenment;

public class Settings
{
    public int LevelReq { get; set; } = 275;
    public int MaxEnlightenments { get; set; } = 5;
    public bool RequireSocietyMaster { get; set; } = true;
    public bool RequireAllLuminanceAuras { get; set; } = true;

    public bool RemoveSociety { get; set; } = true;
    public bool RemoveLuminance { get; set; } = true;
    public bool RemoveAetheria { get; set; } = true;
    public bool RemoveAttributes { get; set; } = true;
    public bool RemoveSkills { get; set; } = true;
    public bool RemoveLevel { get; set; } = true;

    public Dictionary<PropertyInt, int> IntAugments { get; set; } = new()
    {
        [PropertyInt.LumAugCritDamageRating] = 1,
        [PropertyInt.CritRating] = 2,
    };
    public Dictionary<PropertyFloat, double> FloatAugments { get; set; } = new()
    {
        [PropertyFloat.CriticalFrequency] = .01,
    };
    public Dictionary<Skill, int> SkillAugments { get; set; } = new();
    public Dictionary<PropertyAttribute, int> AttributeAugments { get; set; } = new();
    public Dictionary<PropertyAttribute2nd, int> VitalAugments { get; set; } = new();


    //public PropertyBool UsingNewLuminance { get; set; } = (PropertyBool)11111;
    public int SkillCreditAmount { get; set; } = 1;
    public int SkillCreditInterval { get; set; } = 5;
    public bool SkipNormalBroadcast { get; set; } = false;
    public bool SkipResetCertificate { get; set; } = false;
    public long MaxLumBase { get; set; } = 1000000;
    public long MaxLumPerEnlightenment { get; set; } = 500000;

    public bool PatchWieldRequirements { get; set; } = true;
    //public PropertyInt WieldRequirementEnlightenments => (PropertyInt)29999;

    public int BaseLumAugmentationsRequired { get; set; } = 65;
    public int LumAugmentationsRequiredPerEnlightenment { get; set; } = 0;
}