namespace CustomLoot;

//[HarmonyPatch]
//[HarmonyPatchCategory(nameof(Mutator))]
public abstract class Mutator
{
    //Todo: decide on target types
    public Mutation MutationType { get; set; }
    public HashSet<TreasureItemType_Orig>? TargetTypes { get; set; } = new();
    public Odds? Odds { get; set; }


    /// <summary>
    /// Checks for valid target using TargetTypes and successful roll using Odds
    /// </summary>
    public virtual bool Mutates(TreasureDeath profile, TreasureRoll roll, HashSet<Mutation> mutations, WorldObject item = null)
    {
        //Check types
        if (TargetTypes is null)
            return false;

        if (!TargetTypes.Contains(roll.ItemType))
            return false;

        //Check odds
        if (Odds is null)
            return false;

        if (!Odds.TierChance.TryGetValue(profile.Tier, out var chance))
            return false;

        if (ThreadSafeRandom.Next(0f, 1f) >= chance)
            return false;

        return true;
    }

    /// <summary>
    /// Tries to mutate the 
    /// </summary>
    public abstract bool TryMutate(TreasureDeath profile, TreasureRoll roll, HashSet<Mutation> mutations, WorldObject item);

    #region Start/Stop - Placeholders for now
    public virtual void Start()
    {
        //Could leave this to require overriding Start
        //throw new NotImplementedException();
    }

    public virtual void Shutdown()
    {

    }
    #endregion
}
