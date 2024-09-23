namespace AutoLoot.Lib.VTClassic
{
    public interface ILootPluginCapability_SalvageCombineDecision
    {
        bool CanCombineBags(double bag1workmanship, double bag2workmanship, int material);
    }
}