namespace AutoLoot.Lib.VTClassic
{
    public class LootPluginInfo
    {
        internal string a;
        internal string[] b;

        public LootPluginInfo(string ProfileFileExtension, params string[] ExtraDirectories)
        {
            a = ProfileFileExtension.TrimStart('.').ToLowerInvariant();
            if (ExtraDirectories == null)
                b = new string[0];
            else
                b = ExtraDirectories;
        }
    }
}