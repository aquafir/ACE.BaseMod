namespace AutoLoot.Lib.VTClassic
{
    public abstract class LootPluginBase
    {
        public abstract LootPluginInfo Startup();

        public abstract void Shutdown();

        public abstract void LoadProfile(string filename, bool newprofile);

        public abstract void UnloadProfile();

        public abstract void OpenEditorForProfile();

        public abstract void CloseEditorForProfile();

        public abstract bool DoesPotentialItemNeedID(WorldObject item, Player player);

        public abstract LootAction GetLootDecision(WorldObject item, Player player);
    }
}
