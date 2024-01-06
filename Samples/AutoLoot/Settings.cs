namespace AutoLoot;

public class Settings
{
    public string LootProfilePath { get; set; } = Path.Combine(ModManager.ModPath, "LootProfiles");
    public bool LootProfileUseUsername { get; set; } = true;
}