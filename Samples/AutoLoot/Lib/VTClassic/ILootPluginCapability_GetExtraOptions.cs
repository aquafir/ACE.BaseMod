namespace AutoLoot.Lib.VTClassic
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
