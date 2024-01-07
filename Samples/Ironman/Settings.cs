using System.Net;
using System.Text;
using Ironman.FlagEvents;
using Ironman.Restrictions;

namespace Ironman;

public class Settings
{
    public string WelcomeMessage { get; set; } = $"Welcome to Ironman!";
    public int MaxSpecializedSkills { get; set; } = 2;

    public int CreditsToPlanFor { get; set; } = 50;

    public float HardcoreSecondsBetweenDeathAllowed { get; set; } = 60 * 60 * 24 * 7;
    public int HardcoreStartingLives { get; set; } = 1;

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


