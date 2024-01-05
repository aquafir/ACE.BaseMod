namespace AutoLoot;

public class Settings
{
    public string LootProfilePath { get; } = Path.Combine(ModManager.ModPath, "LootProfiles");//Path.Combine(Mod.ModPath, "LootProfiles");
    public bool LootProfileUseUsername { get; set; } = true;
}