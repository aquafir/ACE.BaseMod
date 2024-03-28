namespace uTank2.LootPlugins
{
    [Flags]
    public enum eLootPluginExtraOption
    {
        None = 0,
        HideEditorCheckbox = 1,
    }
    public interface ILootPluginCapability_GetExtraOptions
    {
        eLootPluginExtraOption GetExtraOptions();
    }
}
