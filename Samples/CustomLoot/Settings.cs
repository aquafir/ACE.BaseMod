namespace CustomLoot;

public class Settings
{
    #region Toggles
    //Used by clock mutations
    public bool EnableOnHitForNonCloak { get; set; } = true;
    //Used for aether mutations, like TitaniumWeiner's unique weenies: https://github.com/titaniumweiner/ACEUniqueWeenies
    public bool EnableOnAttackForNonAetheria { get; set; } = false;
    #endregion

    #region Aetheria
    //Enables procs
    public const string OnAttackCategory = "OnAttack";
    #endregion

    #region Sets
    //public bool UseCustomSets { get; set; } = true;
    //Chance of cloak mutation on armor/jewelry/clothes
    public Dictionary<TreasureItemType_Orig, double> SetMutationChance { get; set; } = new()
    {
        [TreasureItemType_Orig.Armor] = .2,
        [TreasureItemType_Orig.Clothing] = .2,
        [TreasureItemType_Orig.Jewelry] = .2,
        [TreasureItemType_Orig.Weapon] = .2,
    };

    //Type -> List of valid eligible sets
    public Dictionary<TreasureItemType_Orig, List<EquipmentSet>> CustomSets { get; set; } = new()
    {
        //Armor / clothes the standard sets
        [TreasureItemType_Orig.Armor] = new(Sets.armorSets) { },
        [TreasureItemType_Orig.Clothing] = new(Sets.armorSets) { },
        //Cloaks / jewelry roll cloak sets
        [TreasureItemType_Orig.Cloak] = new(Sets.cloakSets) { },
        [TreasureItemType_Orig.Jewelry] = new(Sets.cloakSets) { },
        //Weapons do nothing
        //[TreasureItemType_Orig.Weapon] = new() { },   //Ignores missing
    };
    #endregion

    #region Cloak Procs
    public const string OnHitCategory = "OnHit";

    //Chance of cloak mutation on armor/jewelry/clothes
    public Dictionary<TreasureItemType_Orig, double> CloakMutationChance { get; set; } = new()
    {
        [TreasureItemType_Orig.Armor] = .2,
        [TreasureItemType_Orig.Clothing] = .2,
        [TreasureItemType_Orig.Jewelry] = .2,
    };

    //Use custom pool to remove ring / allow other options
    public bool UseCustomCloakSpellProcs { get; set; } = true;
    public List<SpellId> CloakSpells { get; set; } =
       new() {        
           SpellId.VulnerabilityOther8,
           SpellId.AcidArc8
};
        //new (
        ////No ring spells
        ////Sets.cloakSpecificSpells
        ////Add your own
        ////.Append(SpellId.DrainHealth8)
        //);
    #endregion

    #region Slayer
    //Chance of rolling a random slayer
    public double SlayerChance { get; set; } = .25;

    //Power of slayer
    public Dictionary<int, float> SlayerPower { get; set; } = new()
    {
        [0] = .5f,
        [1] = 1f,
        [2] = 1.5f,
        [3] = 2f,
        [4] = 2.5f,
        [5] = 3f,
        [6] = 3.5f,
        [7] = 4f,
        [8] = 4.5f,
    };
    #endregion
}