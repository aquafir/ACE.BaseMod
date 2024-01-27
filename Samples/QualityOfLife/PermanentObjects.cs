namespace QualityOfLife;

[HarmonyPatchCategory(nameof(PermanentObjects))]
public class PermanentObjects
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Landblock), nameof(Landblock.DestroyAllNonPlayerObjects))]
    public static bool PreDestroyAllNonPlayerObjects(ref Landblock __instance)
    {
        __instance.ProcessPendingWorldObjectAdditionsAndRemovals();

        __instance.SaveDB();

        // remove all objects
        var perm = __instance.worldObjects.Values.Where(x => x.GetProperty(FakeBool.Permanent) == true).Select(x => x.Name);
        var p = __instance.players.FirstOrDefault();
        if (p is not null && perm.Count() > 0)
            p.SendMessage($"Permanent items: {String.Join("\n", perm)}");

        foreach (var wo in __instance.worldObjects.Where(i => !(i.Value is Player) && i.Value.GetProperty(FakeBool.Permanent) != true).ToList())
        {
            if (!wo.Value.BiotaOriginatedFromOrHasBeenSavedToDatabase())
                wo.Value.Destroy(false);
            else
                __instance.RemoveWorldObjectInternal(wo.Key);
        }

        __instance.ProcessPendingWorldObjectAdditionsAndRemovals();

        __instance.actionQueue.Clear();

        return false;
    }

}
