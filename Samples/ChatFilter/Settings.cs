namespace ChatFilter;

public class Settings
{
    //Filter list
    //Todo: decide about these being settings?
    public string BlackList = Path.Combine(Mod.Instance.ModPath, "blacklist.txt");
    public string WhiteList = Path.Combine(Mod.Instance.ModPath, "whitelist.txt");
    public bool UseDefaultList { get; set; }

    //Valid filter communication
    public bool FilterTells { get; set; } = true;
    public bool FilterChat { get; set; } = true;
    public const string TellCategory = "Tells";
    public const string ChatCategory = "Chat";

    //Gags
    public bool GagPlayer { get; set; } = true;
    public float GagBaseTime { get; set; } = 60 * 5;
    public float GagTimePerInfraction { get; set; } = 45;
    public bool BroadcastGag { get; set; } = true;

    public bool BanAccount { get; set; } = false;
    public float BanBaseTime { get; set; } = 60 * 60 * 24;
    public float BanTimePerInfraction { get; set; } = 60 * 60 * 24;
    public bool BroadcastBan { get; set; } = true;


    //Users unaware they're muted
    public bool CensorText { get; set; } = true;
    public bool ShadowBan { get; set; } = false;
    //public PropertyBool ShadowBanProperty => (PropertyBool)12345;
    //public PropertyInt ChatInfractionsProperty => (PropertyInt)12345;


}