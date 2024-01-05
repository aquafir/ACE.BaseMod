using System.Collections.Generic;

namespace uTank2.LootPlugins {
    public interface ILootPluginCapability_SalvageCombineDecision2 {
        List<int> ChooseBagsToCombine(List<WorldObject> availablebags);
    }
}