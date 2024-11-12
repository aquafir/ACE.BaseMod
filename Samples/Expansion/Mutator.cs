namespace Expansion;

//[HarmonyPatch]
//[HarmonyPatchCategory(nameof(Mutator))]
public abstract class Mutator
{
    //Todo: decide on target types
    public Mutation MutationType { get; set; }
    public MutationEvent Event { get; set; }
    //Nullity handled on creation by MutatorSettings
    public HashSet<TreasureItemType_Orig> TreasureTargets { get; set; } = new();
    public HashSet<WeenieType> WeenieTypeTargets { get; set; } = new();
    public Odds? Odds { get; set; }


    /// <summary>
    /// Standard checks for valid target using WeenieTypes and successful roll using Odds
    /// </summary>
    public virtual bool CheckMutates(WorldObject item, int tier = 0)
    {
        if (!CheckWeenieTypeTargets(item))
            return false;

        //Roll should be last to avoid inefficiency?
        return CheckRoll(tier);
    }
    /// <summary>
    /// Checks TreasureTargets in addition to WeenieTypes and Tier roll
    /// </summary>
    public virtual bool CheckMutatesLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item = null)
    {
        //Check types
        if (!CheckTreasureTargets(roll))
            return false;

        return CheckMutates(item, profile.Tier);
    }
    protected bool CheckTreasureTargets(TreasureRoll roll)
    {
        if (TreasureTargets is null)
            return true;

        if (!TreasureTargets.Contains(roll.ItemType))
            return false;

        return true;
    }
    protected bool CheckWeenieTypeTargets(WorldObject wo)
    {
        if (WeenieTypeTargets is null)
            return true;

        if (wo is null || !WeenieTypeTargets.Contains(wo.WeenieType))
            return false;

        return true;
    }
    protected bool CheckRoll(int tier = 0)
    {
        //Check odds
        if (Odds is null)
            return true;

        //Get tier / default odds
        if (!Odds.TierChance.TryGetValue(tier, out var chance) && !Odds.TierChance.TryGetValue(0, out chance))
            return false;

        if (ThreadSafeRandom.Next(0f, 1f) >= chance)
            return false;

        return true;
    }

    #region Collection checks to terminate early
    //Check collection to terminate early
    /// <summary>
    /// Stops evaluating Corpse if fails
    /// </summary>
    public virtual bool CanMutateCorpse(DamageHistoryInfo killer = null, Corpse corpse = null, Creature creature = null) => true;
    /// <summary>
    /// Stops evaluating Generator if fails
    /// </summary>
    public virtual bool CanMutateGenerator(GeneratorProfile profile = null) => true;
    /// <summary>
    /// Stops evaluating Vendor if fails
    /// </summary>
    public virtual bool CanMutateVendor(Vendor vendor = null) => true;
    #endregion

    //Todo: decide on throwing an error on fail?
    public virtual bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item) => false;
    public virtual bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item) => false;
    public virtual bool TryMutateGenerator(HashSet<Mutation> mutations, GeneratorProfile generator, WorldObject item) => false;
    public virtual bool TryMutateFactory(HashSet<Mutation> mutations, WorldObject item) => false;
    public virtual bool TryMutateEnterWorld(HashSet<Mutation> mutations, WorldObject item) => false;
    public virtual bool TryMutateEnterInventory(HashSet<Mutation> mutations, WorldObject item, Player player) => false;
    public virtual bool TryMutateEmoteGive(HashSet<Mutation> mutations, WorldObject item, WorldObject giver, Player player) => TryMutateEnterInventory(mutations, item, player);
    public virtual bool TryMutateVendorBuy(HashSet<Mutation> mutations, WorldObject item, Vendor vendor, Player player) => false;

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
