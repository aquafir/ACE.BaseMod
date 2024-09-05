namespace ACE.Shared.Helpers;

//Helpers for mapping from Skill/Attribute/Vital to related bonus props
public static class BonusPropExtensions
{
    const int SKILL_START = (int)(FakeInt.BonusAxe - 1);
    const int ATTR_START = (int)(FakeInt.BonusStrength - 1);
    const int VITAL_START = (int)(FakeInt.BonusMaxHealth - 1);

    //Gets the FakeProp corresponding to a flat bonus to a Skill/Attribute/Vital
    public static PropertyInt GetBonusProp(this Skill skill) => (PropertyInt)(SKILL_START + skill);
    public static PropertyInt GetBonusProp(this PropertyAttribute attribute) => (PropertyInt)(ATTR_START + attribute);
    public static PropertyInt GetBonusProp(this PropertyAttribute2nd vital) => (PropertyInt)(VITAL_START + vital);

    public static int GetBonus(this Creature creature, Skill key) =>
        creature.GetProperty(key.GetBonusProp()) ?? 0;
    public static void IncBonus(this Creature creature, Skill key, int change) =>
        creature.SetBonus(key, creature.GetBonus(key) + change);
    public static void SetBonus(this Creature creature, Skill key, int value) =>
        creature.SetProperty(key.GetBonusProp(), value);

    public static int GetBonus(this Creature creature, PropertyAttribute key) =>
        creature.GetProperty(key.GetBonusProp()) ?? 0;
    public static void IncBonus(this Creature creature, PropertyAttribute key, int change) =>
        creature.SetBonus(key, creature.GetBonus(key) + change);
    public static void SetBonus(this Creature creature, PropertyAttribute key, int value) =>
        creature.SetProperty(key.GetBonusProp(), value);

    public static int GetBonus(this Creature creature, PropertyAttribute2nd key) =>
        creature.GetProperty(key.GetBonusProp()) ?? 0;
    public static void IncBonus(this Creature creature, PropertyAttribute2nd key, int change) =>
        creature.SetBonus(key, creature.GetBonus(key) + change);
    public static void SetBonus(this Creature creature, PropertyAttribute2nd key, int value) =>
        creature.SetProperty(key.GetBonusProp(), value);

    //Send and update
    public static void SetBonus(this Player player, Skill key, int value)
    {
        player.SetProperty(key.GetBonusProp(), value);
        player.SendUpdated(key);
    }
    public static void SetBonus(this Player player, PropertyAttribute key, int value)
    {
        player.SetProperty(key.GetBonusProp(), value);
        player.SendUpdated(key);
    }
    public static void SetBonus(this Player player, PropertyAttribute2nd key, int value)
    {
        player.SetProperty(key.GetBonusProp(), value);
        player.SendUpdated(key);
    }
}
