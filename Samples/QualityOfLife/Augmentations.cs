using System.Runtime.CompilerServices;

namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Features.Augmentations))]
public class Augmentations
{
    public static AugmentationSettings Settings => PatchClass.Settings.Augmentation;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AugTypeHelper), nameof(AugTypeHelper.IsResist), new Type[] { typeof(AugmentationType) })]
    public static void PostIsResist(AugmentationType type, ref bool __result)
    {
        __result = Settings.IgnoreSharedResist? false : __result;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AugTypeHelper), nameof(AugTypeHelper.IsAttribute), new Type[] { typeof(AugmentationType) })]
    public static void PostIsAttribute(AugmentationType type, ref bool __result)
    {
        __result = Settings.IgnoreSharedAttribute ? false : __result;
    }

    public static void OverrideCaps()
    {
        foreach(var kvp in Settings.MaxAugs)
        {
            if (AugmentationDevice.MaxAugs.ContainsKey(kvp.Key))
                AugmentationDevice.MaxAugs[kvp.Key] = kvp.Value;
        }
    }
}

public class AugmentationSettings
{
    public bool IgnoreSharedAttribute { get; set; } = false;
    public bool IgnoreSharedResist { get; set; } = false;

    //Patched on startup if Feature enabled to override maxs
    public Dictionary<AugmentationType, int> MaxAugs = new Dictionary<AugmentationType, int>()
    {
        { AugmentationType.Strength, 10 },          // attributes in shared group unless overridden
        { AugmentationType.Endurance, 10 },
        { AugmentationType.Coordination, 10 },
        { AugmentationType.Quickness, 10 },
        { AugmentationType.Focus, 10 },
        { AugmentationType.Self, 10 },
        { AugmentationType.Salvage, 1 },
        { AugmentationType.ItemTinkering, 1 },
        { AugmentationType.ArmorTinkering, 1 },
        { AugmentationType.MagicItemTinkering, 1 },
        { AugmentationType.WeaponTinkering, 1 },
        { AugmentationType.PackSlot, 1 },
        { AugmentationType.BurdenLimit, 5 },
        { AugmentationType.DeathItemLoss, 3 },
        { AugmentationType.DeathSpellLoss, 1 },
        { AugmentationType.CritProtect, 1 },
        { AugmentationType.BonusXP, 1 },
        { AugmentationType.BonusSalvage, 4 },
        { AugmentationType.ImbueChance, 1 },
        { AugmentationType.RegenBonus, 2 },
        { AugmentationType.SpellDuration, 5 },
        { AugmentationType.ResistSlash, 2 },
        { AugmentationType.ResistPierce, 2 },
        { AugmentationType.ResistBludgeon, 2 },
        { AugmentationType.ResistAcid, 2 },
        { AugmentationType.ResistFire, 2 },
        { AugmentationType.ResistCold, 2 },
        { AugmentationType.ResistElectric, 2 },
        { AugmentationType.FociCreature, 1 },
        { AugmentationType.FociItem, 1 },
        { AugmentationType.FociLife, 1 },
        { AugmentationType.FociWar, 1 },
        { AugmentationType.CritChance, 1 },
        { AugmentationType.CritDamage, 1 },
        { AugmentationType.Melee, 1 },
        { AugmentationType.Missile, 1 },
        { AugmentationType.Magic, 1 },
        { AugmentationType.Damage, 1 },
        { AugmentationType.DamageResist, 1 },
        { AugmentationType.AllStats, 1 },
        { AugmentationType.FociVoid, 1 },
    };
}