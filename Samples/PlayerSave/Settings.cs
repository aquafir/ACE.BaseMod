namespace Tinkering
{
    public class Settings
    {
        public string SaveDirectory { get; internal set; } = Path.Combine(Mod.ModPath, "Saves");

        public LoadOptions Options { get; set; } = LoadOptions.Default;

        public static string Extension => ".acesave";
    }
}