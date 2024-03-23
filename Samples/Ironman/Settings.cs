using Ironman.FlagEvents;
using Ironman.Restrictions;

namespace Ironman;

public class Settings
{
    public string WelcomeMessage { get; set; } = $"Welcome to Ironman!";
    public int MaxSpecializedSkills { get; set; } = 2;

    public string StartingLocation { get; set; } = "0xA9B40019 [84.000000 7.100000 94.005005] 0.996917 0.000000 0.000000 -0.078459";

    public int CreditsToPlanFor { get; set; } = 50;

    public float HardcoreSecondsBetweenDeathAllowed { get; set; } = 60 * 60 * 24 * 7;
    public int HardcoreStartingLives { get; set; } = 1;
    public bool QuarantineOnDeath { get; set; } = true;

    public List<string> Restrictions { get; set; } = new()
    {
        nameof(ForceSkillPlan),
        nameof(ForceTemplatePlayer),
        nameof(Hardcore),
        nameof(RestrictAllegiance),
        nameof(RestrictContainerVerify),
        nameof(RestrictCreateInInventory),
        nameof(RestrictEnchantments),
        nameof(RestrictEquip),
        nameof(RestrictFellowship),
        //nameof(),
    };
    public List<string> FlagItemEvents { get; set; } = new()
    {
        nameof(FlagChest),
        nameof(FlagCorpseItems),
        nameof(FlagEmoteItems),
        nameof(FlagVendorItems),
        //nameof(),
    };

    public Dictionary<Skill, List<string>> SkillItems = new()
    {
        [Skill.WarMagic] = new()
        {
            "12748",        //Wand
            "20631 100",    //Tapers
            "691 10",       //Lead scarab
        },
        [Skill.VoidMagic] = new()
        {
            "12748",        //Wand
            "20631 100",    //Tapers
            "691 10",       //Lead scarab
        }
    };

    //Determines
    public bool UseSpellList { get; set; } = true;
    public List<SpellId> DefaultSpells { get; set; } = new()
    {
        //Creature
        SpellId.FocusSelf1,
        SpellId.InvulnerabilityOther1,
        SpellId.InvulnerabilitySelf1,
        //Life
        SpellId.ArmorOther1,
        SpellId.ArmorSelf1,
        SpellId.HealOther1,
        SpellId.HealSelf1,
        SpellId.ImperilOther1,
        //Item
        SpellId.BloodDrinkerSelf1,
        SpellId.SwiftKillerSelf1,
        SpellId.BludgeonBane1,
        SpellId.Impenetrability1,
        //Void
        SpellId.Corrosion1,
        SpellId.Corruption1,
        SpellId.CurseDestructionOther1,
        SpellId.NetherBolt1,
        //War
        SpellId.FlameBolt1,
        SpellId.FrostBolt1,
        SpellId.ShockWave1,
        SpellId.ForceBolt1,
    };

    public List<Skill> PrimarySkillPool { get; set; } = new()
    {
        Skill.TwoHandedCombat,
        Skill.MissileWeapons,
        Skill.WarMagic,
        Skill.VoidMagic,
        Skill.LightWeapons,
        Skill.HeavyWeapons,
        Skill.FinesseWeapons,
    };

    public static readonly HashSet<Skill> AugmentSpecializations = new() {
        Skill.Salvaging,
        Skill.ArmorTinkering,
        Skill.ItemTinkering,
        Skill.MagicItemTinkering,
        Skill.WeaponTinkering,
    };

    public List<Skill> SecondarySkillPool { get; set; } = new()
    {
        //No dupes
        //Skill.TwoHandedCombat,
        //Skill.MissileWeapons,
        //Skill.WarMagic,
        //Skill.VoidMagic,
        //Skill.LightWeapons,
        //Skill.HeavyWeapons,
        //Skill.FinesseWeapons,

        //Pretrained
        //Skill.ArcaneLore,
        //Skill.Jump,
        //Skill.Loyalty,
        //Skill.MagicDefense,
        //Skill.Run,
        //Skill.Salvaging,

        Skill.Alchemy,
        Skill.ArmorTinkering,
        Skill.AssessCreature,
        Skill.AssessPerson,
        Skill.Cooking,
        Skill.CreatureEnchantment,
        Skill.Deception,
        Skill.DirtyFighting,
        Skill.DualWield,
        Skill.Fletching,
        Skill.Healing,
        Skill.ItemEnchantment,
        Skill.ItemTinkering,
        Skill.Leadership,
        Skill.LifeMagic,
        Skill.Lockpick,
        Skill.MagicItemTinkering,
        Skill.ManaConversion,
        Skill.MeleeDefense,
        Skill.MissileDefense,
        Skill.Recklessness,
        Skill.Shield,
        Skill.SneakAttack,
        Skill.Summoning,
        Skill.WeaponTinkering,
    };
}


