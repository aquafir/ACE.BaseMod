namespace CustomSpells;


public class Settings
{
    public bool AutoloadSpreadsheet { get; set; } = true;
    public string Spreadsheet { get; set; } = Path.Combine(Mod.Instance.ModPath, "Spells.xlsx");

    /// <summary>
    /// Custom Spells use a template, make modifications, then add the custom spell as the Id
    /// </summary>
    public List<SpellCustomization> CustomSpells { get; set; } = new()
    {
        new(SpellId.StrengthSelf1, SpellId.StrengthSelf1, Name: "Lesser Strength I", StatModVal: 5, TargetEffect: PlayScript.LevelUp),
        new(SpellId.StrengthSelf1, (SpellId)9999, Name: "Boosted Strength I", StatModVal: 111, Category: (SpellCategory)987),
        new(SpellId.AcidVolley1, SpellId.AcidVolley1, EType: DamageType.Bludgeon, BaseIntensity: 50, Variance: 0),
    };

    public Dictionary<EquipmentSet, List<SetTier>> Sets { get; set; } = new()
    {
        [EquipmentSet.Adepts] = new()
        {
            new(3, new()
            {
                SpellId.HarmSelf3,
                SpellId.AcidProtectionSelf3,
            }),
            new(4, new()
            {
                SpellId.HarmSelf5,
                SpellId.AcidProtectionSelf5,
            }),
        },
        [(EquipmentSet)999] = new()
        {
            new(1, new()
            {
                SpellId.HarmSelf6,
                SpellId.FireProtectionSelf6,
            }),
            new(2, new()
            {
                SpellId.HarmSelf7,
                SpellId.FireProtectionSelf7,
            }),
        },
    };
}

public record struct SetTier(uint NumEquipped, List<SpellId> Spells);
