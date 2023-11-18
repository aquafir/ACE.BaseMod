namespace ChatFilter;

public class Settings
{
    // Your settings here
    public string BlackList { get; set; } = "blacklist.txt";
    public string WhiteList { get; set; } = "whitelist.txt";

    //Loads defaults from https://github.com/stephenhaunts/ProfanityDetector/blob/main/ProfanityFilter/ProfanityFilter/ProfanityList.cs
    //Additional list courtesy of Maethor: https://github.com/surge-ai/profanity
    public bool UseDefaultList { get; set; }

    //Users unaware they're muted
    public bool ShadowBan { get; set; }
    public PropertyBool ShadowBanProperty => (PropertyBool)12345;
}