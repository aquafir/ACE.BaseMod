namespace PlayerSave
{
    public class Settings
    {
        public string SaveDirectory { get; internal set; } = Path.Combine(Mod.Instance.ModPath, "Saves");

        public LoadOptions Options { get; set; } = LoadOptions.Default;

        public static string Extension => ".acesave";
    }
}