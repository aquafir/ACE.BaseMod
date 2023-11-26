namespace CustomLoot;

public class Settings
{
    public bool Verbose { get; set; } = false;

    #region Features / Mutators
    public List<Feature> Features { get; set; } = Enum.GetValues<Feature>().ToList();
    public List<MutatorSettings> Mutators { get; set; } =
        //Select items
        new()
        {
            new MutatorSettings(Mutation.GrowthItem) {
            Odds = nameof(OddsGroup.Always),
            Targets = nameof(TargetGroup.Weapon)
        }
        };
    //Full set
    //Enum.GetValues<Mutation>()
    //.Select(x => new MutatorSettings(x) {
    //    Odds = x.DefaultOdds(),
    //    Targets = x.DefaultTargets()
    //}).ToList();
    #endregion

    #region Mutator Settings
    #region GrowthItem
    //Type -> List of eligible augments on growth
    public Dictionary<TreasureItemType_Orig, string> GrowthAugments { get; set; } = new()
    {
        [TreasureItemType_Orig.Weapon] = nameof(AugmentGroup.Weapon),
        [TreasureItemType_Orig.Armor] = nameof(AugmentGroup.Armor),
    };
    //Type->Level-fixed augments.  Should it be groups instead?
    public Dictionary<TreasureItemType_Orig, Dictionary<int,Augment>> GrowthFixedLevelAugments { get; set; } = new()
    {
        [TreasureItemType_Orig.Weapon] = new Dictionary<int, Augment>()
        {
            [1] = Augment.RendAll,
        }        
    };
    //Tier->Low/high level range
    public Dictionary<int, IntRange> GrowthTierLevelRange { get; set; } = Enumerable.Range(1, 8).ToDictionary(x => x, x => Range.Int(x * 2 - 1, x * 2 + 3));
    //Tier->XP.  Defaulting to same cost per level
    public long GrowthXpBase { get; set; } = 1_000_000;
    public double GrowthXpScaleByTier { get; set; } = 1.2;
    //public Dictionary<int, long> GrowthTierXpCost => Enumerable.Range(1, 8).ToDictionary(x => x, x => (long)(1000000 * Math.Pow(1.5, x)));

    #endregion

    #region Set
    //Type -> List of eligible sets
    public Dictionary<TreasureItemType_Orig, EquipmentSetGroup> ItemTypeEquipmentSets { get; set; } = new()
    {
        //Armor / clothes the standard sets
        [TreasureItemType_Orig.Armor] = EquipmentSetGroup.Armor,
        [TreasureItemType_Orig.Clothing] = EquipmentSetGroup.Armor,
        //Cloaks / jewelry roll cloak sets
        [TreasureItemType_Orig.Cloak] = EquipmentSetGroup.Cloak,
        [TreasureItemType_Orig.Jewelry] = EquipmentSetGroup.Cloak,
        //Weapons do nothing
        //[TreasureItemType_Orig.Weapon] = new() { },   //Ignores missing
    };
    #endregion

    #region Slayer
    public string Slayers { get; set; } = nameof(CreatureTypeGroup.Popular);

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

    #region ProcOnHit
    //Use custom pool to remove ring / allow other options
    public string ProcOnSpells { get; set; } = nameof(SpellGroup.CloakOnly);
    #endregion
    #endregion

    #region Feature Settings

    #region ProcRateOverride
    public double CloakProcRate { get; set; } = .05; //5%
    public float AetheriaProcRate { get; set; } = .05f;
    #endregion
    #endregion

    #region Pools (e.g., Odds / Targets / Sets / Spell IDs / Species)
    //For convenience.  People can make their own
    public Dictionary<string, Odds> Odds { get; set; } = Enum.GetValues<OddsGroup>().ToDictionary(x => x.ToString(), x => x.OddsOf());
    public Dictionary<string, TreasureItemType_Orig[]> TargetGroups { get; set; } = new()
    {
        [nameof(TargetGroup.Accessories)] = TargetGroup.Accessories.SetOf(),
        [nameof(TargetGroup.ArmorClothing)] = TargetGroup.ArmorClothing.SetOf(),
        [nameof(TargetGroup.Equipables)] = TargetGroup.Equipables.SetOf(),
        [nameof(TargetGroup.Weapon)] = TargetGroup.Weapon.SetOf(),
        [nameof(TargetGroup.Wearables)] = TargetGroup.Wearables.SetOf(),
    };
    //Full pools defined in enum helpers or it can be done explicitly like TargetGroups
    public Dictionary<string, CreatureType[]> CreatureTypeGroups { get; set; } = Enum.GetValues<CreatureTypeGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, ImbuedEffectType[]> ImbueGroups { get; set; } = Enum.GetValues<ImbueGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, EquipmentSet[]> EquipmentSetGroups { get; set; } = Enum.GetValues<EquipmentSetGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, SpellId[]> SpellGroups { get; set; } = Enum.GetValues<SpellGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, Augment[]> AugmentGroups { get; set; } = Enum.GetValues<AugmentGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    #endregion
}
