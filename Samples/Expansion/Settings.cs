namespace Expansion;

public class Settings
{
    public bool Verbose { get; set; } = false;

    #region Features / Mutators
    public List<Feature> Features { get; set; } = new()
    {
        //Feature.FakePropertyCache,
        Feature.CreatureEx,
        Feature.MutatorHooks, 
    };

    public List<CreatureExType> CreatureFeatures { get; set; } = new()
    {
        CreatureExType.Horde,
        //CreatureExType.Accurate,
        //CreatureExType.Boss,
        //CreatureExType.Comboer,
        //CreatureExType.Drainer,
        //CreatureExType.Evader,
        //CreatureExType.Rogue,
        //CreatureExType.SpellBreaker,
        //CreatureExType.SpellThief,
        //CreatureExType.Stomper,
        //CreatureExType.Tank,
        //CreatureExType.Vampire,
        //CreatureExType.Warder,
    };

    //Full set
    public string MutatorPath => Path.Combine(Mod.Instance.ModPath, "Mutators");
    public List<MutatorSettings> Mutators { get; set; } =
        //Select items
        new()
        {
            new MutatorSettings(Mutation.SampleMutator.ToString()) {
            Odds = nameof(OddsGroup.Always),
            Events = MutationEvent.Containers | MutationEvent.EmoteGive
            },
            //new MutatorSettings(Mutation.Enlightened) {
            //Odds = nameof(OddsGroup.Always),
            //Events = MutationEvent.Containers | MutationEvent.EnterWorld
            //},
            //new MutatorSettings(Mutation.IronmanLocked)
            //{
            //    Events = MutationEvent.Containers,
            //}
            //new MutatorSettings(Mutation.GrowthItem) {
            //Odds = nameof(OddsGroup.Always),
            //TreasureTargets = nameof(TargetGroup.Weapon),
            //Events = MutationEvent.Containers
            //},
            //new MutatorSettings(Mutation.Resize)  {
            //Events = Mutation.Resize.DefaultEvents(),
            //Odds = null,
            //},
        };
    //Full set
    //Enum.GetValues<Mutation>()
    //.Select(x => new MutatorSettings(x)
    //{
    //    Odds = x.DefaultOdds(),
    //    TreasureTargets = x.DefaultTargets()
    //}).ToList();
    #endregion

    #region Mutator Settings
    #region GrowthItem
    //Type -> List of eligible augments on growth
    public Dictionary<WeenieType, string> GrowthAugments { get; set; } = new()
    {
        [WeenieType.MeleeWeapon] = nameof(AugmentGroup.Weapon),
        [WeenieType.Caster] = nameof(AugmentGroup.Weapon),
        [WeenieType.MissileLauncher] = nameof(AugmentGroup.Weapon),
        [WeenieType.Clothing] = nameof(AugmentGroup.Armor),
    };
    //Type->Level-fixed augments.  Should it be groups instead?
    public Dictionary<WeenieType, Dictionary<int, Augment>> GrowthFixedLevelAugments { get; set; } = new()
    {
        [WeenieType.MeleeWeapon] = new Dictionary<int, Augment>()
        {
            [1] = Augment.RendAll,
        },
        [WeenieType.Caster] = new Dictionary<int, Augment>()
        {
            [1] = Augment.RendAll,
        },
        [WeenieType.MissileLauncher] = new Dictionary<int, Augment>()
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
    #region FakePropertyCache
    public PropertyBonusSettings BonusCaps { get; set; } = new();

    #endregion

    #region ProcRateOverride
    public double CloakProcRate { get; set; } = .05; //5%
    public float AetheriaProcRate { get; set; } = .05f;
    #endregion

    #region AutoLoot
    public string LootProfilePath { get; } = Path.Combine(ModManager.ModPath, "LootProfiles");//Path.Combine(Mod.ModPath, "LootProfiles");
    public bool LootProfileUseUsername { get; set; } = true;
    #endregion

    #region Hardcore
    public float HardcoreSecondsBetweenDeathAllowed { get; set; } = 60;
    public int HardcoreStartingLives { get; set; } = 5;

    #endregion
    #endregion

    public double CreatureChance { get; set; } = 0;

    public SpellSettings SpellSettings { get; set; } = new();

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
    public Dictionary<string, WeenieType[]> WeenieTypeGroups { get; set; } = new()
    {
        [nameof(WeenieTypeGroup.Container)] = WeenieTypeGroup.Container.SetOf(),
    };
    //Full pools defined in enum helpers or it can be done explicitly like TargetGroups
    public Dictionary<string, CreatureType[]> CreatureTypeGroups { get; set; } = Enum.GetValues<CreatureTypeGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, EquipmentSet[]> EquipmentSetGroups { get; set; } = Enum.GetValues<EquipmentSetGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, SpellId[]> SpellGroups { get; set; } = Enum.GetValues<SpellGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    public Dictionary<string, Augment[]> AugmentGroups { get; set; } = Enum.GetValues<AugmentGroup>().ToDictionary(x => x.ToString(), x => x.SetOf());
    #endregion
}
