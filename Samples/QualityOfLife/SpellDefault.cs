
using ACE.Database;

namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Patches.SpellDefault))]
public class SpellDefault
{
    public static void SetupSpells()
    {
        //Cache spells in case they aren't already?
        //DatabaseManager.World.CacheAllSpells();

        List<SpellId> spells = new() { 
            SpellId.ParagonQuicknessI,
            SpellId.ParagonsCriticalBoostI,
        };       

        foreach(var spellId in spells)
        {
            var spell = new Spell(spellId);
            spell.Duration
            spellId.
        }
        Spell.
    }
}

public class SpellDefaultSettings
{
    public float PowerLow { get; set; } = .2f;
    public float PowerHigh { get; set; } = .8f;
    public int RatingTrained { get; set; } = 10;
    public int RatingSpecialized { get; set; } = 20;
}