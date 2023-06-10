using Saves.Save;

namespace Saves;

public class Settings
{
    public string SaveDirectory { get; internal set; } = Path.Combine(Mod.ModPath, "wwwroot", "saves");

    public LoadOptions Options { get; set; } = LoadOptions.Default;

    public static string Extension => ".acesave";
}