namespace Expansion.Enums;


public enum SpellGroup
{
    Cloak,
    CloakOnly,
    CloakRings,
    Aetheria,
}

public static class SpellGroupHelper
{
    public static SpellId[] SetOf(this SpellGroup type) => type switch
    {
        SpellGroup.Cloak =>
        SpellGroup.CloakOnly.SetOf()
            .AddRangeToArray(SpellGroup.CloakOnly.SetOf()),
        SpellGroup.CloakOnly => new[]
        {
            SpellId.CloakAllSkill,      // Cloaked in Skill
            SpellId.CloakMagicDLower,   // Shroud of Darkness (Magic)
            SpellId.CloakMeleeDLower,   // Shroud of Darkness (Melee)
            SpellId.CloakMissileDLower, // Shroud of Darkness (Missile)
        },
        SpellGroup.CloakRings => new[]
        {
            SpellId.AcidRing,           // Searing Disc
            SpellId.BladeRing,          // Horizon's Blades
            SpellId.FlameRing,          // Cassius' Ring of Fire
            SpellId.ForceRing,          // Nuhmudira's Spines
            SpellId.FrostRing,          // Halo of Frost
            SpellId.LightningRing,      // Eye of the Storm
            SpellId.ShockwaveRing,      // Tectonic Rifts
            SpellId.NetherRing,         // Clouded Soul
        },
        SpellGroup.Aetheria => new[]
        {
            SpellId.AetheriaProcDamageBoost,
            SpellId.AetheriaProcDamageOverTime,
            SpellId.AetheriaProcDamageReduction,
            SpellId.AetheriaProcHealDebuff,
            SpellId.AetheriaProcHealthOverTime,
        },
        _ => throw new NotImplementedException(),
    };
}
