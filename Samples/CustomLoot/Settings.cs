using System.Linq;

namespace CustomLoot;

public class Settings
{
    #region Features / Mutators / Odds / Targets
    public List<Feature> Features { get; set; } = Enum.GetValues<Feature>().ToList();
    public List<MutatorSettings> Mutators { get; set; } = Enum.GetValues<Mutation>().Select(x => new MutatorSettings(x.ToString())).ToList();

    //OddsType and helpers just for convenience-- any string can be used
    public Dictionary<string, Odds> Odds { get; set; } = new()
    {
        [nameof(OddsType.Common)] = OddsType.Common.OddsOf(),
        [nameof(OddsType.Rare)] = OddsType.Rare.OddsOf(),
        [nameof(OddsType.Always)] = OddsType.Always.OddsOf(),
    };

    //For convenience.  People can make their own
    public Dictionary<string, HashSet<TreasureItemType_Orig>> TargetGroups { get; set; } = new()
    {
        [nameof(TargetGroup.Accessories)] = TargetGroup.Accessories.SetOf(),
        [nameof(TargetGroup.ArmorClothing)] = TargetGroup.ArmorClothing.SetOf(),
        [nameof(TargetGroup.Equipables)] = TargetGroup.Equipables.SetOf(),
        [nameof(TargetGroup.Weapon)] = TargetGroup.Weapon.SetOf(),
        [nameof(TargetGroup.Wearables)] = TargetGroup.Wearables.SetOf(),

        //[nameof(TargetGroup.Armor)] = TargetGroup.Armor.SetOf(),
        //[nameof(TargetGroup.Cloaks)] = TargetGroup.Cloaks.SetOf(),
        //[nameof(TargetGroup.Clothing)] = TargetGroup.Clothing.SetOf(),
        //[nameof(TargetGroup.Consumable)] = TargetGroup.Consumable.SetOf(),
        //[nameof(TargetGroup.Jewelry)] = TargetGroup.Jewelry.SetOf(),
        //[nameof(TargetGroup.Pet)] = TargetGroup.Pet.SetOf(),
    };
    #endregion

    #region Mutator Settings
    #region Slayer
    //Default list is all defined except wall
    public CreatureType[] SlayerSpecies { get; set; } = Enum.GetValues<CreatureType>();
    //.TakeWhile(x => x != CreatureType.Unknown && x != CreatureType.Wall && x != CreatureType.Invalid).ToArray();

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


    #endregion

    #region Feature Settings



    #region ProcRateOverride
    public double CloakProcRate { get; set; } = .05; //5%
    public double AetheriaProcRate { get; set; } = .05;
    #endregion
    #endregion


    #region Sets
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
    //Use custom pool to remove ring / allow other options
    public bool UseCustomCloakSpellProcs { get; set; } = true;
    public List<SpellId> CloakSpells { get; set; } =
        new(
        Sets.cloakSpecificSpells            //No ring spells
        .Append(SpellId.DrainHealth8)       //Add your own
        );
    #endregion


}
