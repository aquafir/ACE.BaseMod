namespace CustomLoot;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(Mutator))]
public abstract class Mutator
{
    //Todo: decide on target types
    public HashSet<TreasureItemType_Orig>? TargetTypes { get; set; } = new();
    public Odds? Odds { get; set; } 

    public virtual bool Mutates()
    {
        return true;
    }

    public virtual bool TryMutate()
    {
        return true;
    }


    #region Start/Stop
    public virtual void Start()
    {
        throw new NotImplementedException();
    }

    public virtual void Shutdown()
    {
        //Per-category patches don't exist in the same way.
        //Clean up objects.  If you do something like null func you'll have to check for nulls when using it in patches
    }
    #endregion
}
