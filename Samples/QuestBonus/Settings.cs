namespace QuestBonus;

public class Settings
{
    //What PropertyFloat to store the bonus under
    //public PropertyFloat QuestBonusProperty { get; set; } = (PropertyFloat)5678;

    //QB Point to XP multiplier rate
    public float BonusConversion { get; set; } = 1.0f / 20;

    //Default QB
    public float DefaultPoints { get; set; } = 1;

    //Quests tell you when they add xp
    public bool NotifyQuest { get; set; } = true;

    //Shows xp bonus on xp gain
    public bool NotifyExp { get; set; } = false;

    //Quest Name modifier
    public Dictionary<string, float> QuestBonuses { get; set; } = new()
    {
        ["PathwardenComplete"] = 10,
        ["PathwardenFound1111"] = 5,
        ["StipendsCollectedInAMonth"] = 0,
        ["StipendTimer_08"] = 0,
        ["StipendTimer_Monthly"] = 0,
    };
}