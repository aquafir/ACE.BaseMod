namespace uTank2.LootPlugins {
    public class LootPluginInfo {
        internal string a;
        internal string[] b;

        public LootPluginInfo(string ProfileFileExtension, params string[] ExtraDirectories) {
            this.a = ProfileFileExtension.TrimStart('.').ToLowerInvariant();
            if (ExtraDirectories == null)
                this.b = new string[0];
            else
                this.b = ExtraDirectories;
        }
    }
}