namespace Tower;

public class Settings
{
    public LootStyle LootStyle { get; set; } = LootStyle.RoundRobin;
    public LooterRequirements LooterRequirements { get; set; } = LooterRequirements.Range;
    public ChatMessageType MessageType { get; set; } = ChatMessageType.Broadcast;

    public bool VendorsUseBank { get; set; } = true;
    //Doesn't print blank banked items
    public bool SkipMissingBankedItems { get; set; } = true;

    //Reduces amount to a cap
    public bool ExcessSetToMax { get; set; } = true;

    //public bool SendRequiresExactName { get; set; } = true;
    //public bool SendRequiresSameAccount { get; set; } = true;

    //WCID - PropInt64
    public List<BankItem> Items { get; set; } = new()
    {
        new ("MMD", 20630, 40000),
        new ("Infused Amber Shard",52968, 40001),
        new ("Small Olthoi Venom Sac", 36376, 40002),
        new ("A'nekshay Token", 44240, 40003),
        new ("Ornate Gear Marker", 43142, 40004),
        new ("Colosseum Coin",36518, 40005),
        new ("Ancient Mhoire Coin", 35383, 40006),
        new ("Promissory Note", 43901, 40007),
    };

    public bool XpBonusEnabled { get; set; } = true;
    public float MaxXpBonus { get; set; } = 5;
    public float MaxXpBonusLevelRange { get; set; } = 30;

    public bool LootBonusEnabled { get; set; } = true;
    public float MaxLootBonus { get; set; } = 2;
    public float MaxLootBonusLevelRange { get; set; } = 30;

    public OfflineProgressSettings OfflineProgress { get; set; } = new();

    public List<TowerFloor> Floors { get; set; } = new()
    {
//016C : MP non instance
//5369 : Housing non instance
//5950 : Base non instance
//5F44 : Jungle Base non instance
        new("Floor 1", 0, 0x019E, 10, 0x0159),
        new("Floor 2", 1, 0x0159, 20, 0x013B),
        new("Floor 3", 2, 0x013B, 30, 0x6345),
        new("Floor 4", 3, 0x6345, 40, 0x009D),
        new("Floor 5", 4, 0x009D, 50, 0x03A5),
        new("Floor 6", 5, 0x03A5, 60, 0x5F44),
        new("Floor 7", 6, 0x01A3, 70, 0x016A),
        new("Floor 7.5", 7, 0x016A, 80, 0x0110),
        new("Floor 8", 8, 0x0110, 90, 0x0010),
        new("Floor 9", 9, 0x0010, 100, 0x013C),
        new("Floor 10", 10, 0x013C, 110, 0x001A),
        new("Floor 11", 11, 0x001A, 120, 0x002D),
        new("Floor 12", 12, 0x002D, 130, 0x0000),
    };


    public List<FistPool> FistMagicPools { get; set; } = new()
    {
        new() {
            LimitingSkill = Skill.WarMagic,
            Spells = new()
              {
                  new SkillSpellPair(0, SpellId.LightningArc1, 0),
                  new SkillSpellPair(94, SpellId.LightningArc2, 0),
                  new SkillSpellPair(141, SpellId.LightningArc3, 0),
                  new SkillSpellPair(188, SpellId.LightningArc4, 0),
                  new SkillSpellPair(235, SpellId.LightningArc5, 0),
                  new SkillSpellPair(282, SpellId.EyeOfTheStormII, 0),
              }
        },
        new() {
            LimitingSkill = Skill.LifeMagic,
            Spells = new()
              {
                  new SkillSpellPair(94, SpellId.LightningVulnerabilityOther1, 0),
                  new SkillSpellPair(141, SpellId.LightningVulnerabilityOther2, 0),
                  new SkillSpellPair(188, SpellId.LightningVulnerabilityOther3, 0),
                  new SkillSpellPair(235, SpellId.LightningVulnerabilityOther4, 0),
                  new SkillSpellPair(282, SpellId.LightningVulnerabilityOther5, 0),
                  new SkillSpellPair(329, SpellId.LightningVulnerabilityOther6, 0),
              }
        },
    };

}

//public record struct TowerFloor(string Name, int Index, ushort Landblock, uint Level);
//public record struct BankItem(string Name, uint Id, int Prop);

public record TowerFloor(string Name, int Index, ushort Landblock, ushort ExitLandblock, uint Level);
public record BankItem(string Name, uint Id, int Prop);


