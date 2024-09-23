namespace QuestBonus;
public static class QuestBonusExtensions
{
    /// <summary>
    /// Gets weighted or default QuestPoints
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public static float Value(this CharacterPropertiesQuestRegistry quest) =>
        PatchClass.Settings.QuestBonuses.TryGetValue(quest.QuestName, out var points) ? points : PatchClass.Settings.DefaultPoints;

    /// <summary>
    /// Updates QuestPoints with completed quests multiplied by weight
    /// </summary>
    /// <param name="player"></param>
    public static void UpdateQuestPoints(this Player player) => player.SetProperty(FakeFloat.QuestBonus, player.CalculateQuestPoints());

    /// <summary>
    /// Adds an amount to current QuestPoints
    /// </summary>
    public static void IncQuestPoints(this Player player, float amount)
    {
        var current = player.GetProperty(FakeFloat.QuestBonus) ?? 0;

        player.SetProperty(FakeFloat.QuestBonus, current + amount);
    }

    /// <summary>
    /// Sums completed quests weighted to get total QuestPoints
    /// </summary>
    public static float CalculateQuestPoints(this Player player)
    {
        float total = 0;
        foreach (var quest in player.QuestManager.GetQuests().Where(x => x.HasSolves())) //x.NumTimesCompleted > 0))
            total += quest.Value();


        return (float)total;
    }

    /// <summary>
    /// Quest Points adjusted by bonus multiplier
    /// </summary>
    public static double QuestBonus(this Player player)
    {
        var qb = player.GetProperty(FakeFloat.QuestBonus) ?? 0;
        return 1 + qb * PatchClass.Settings.BonusConversion;
    }
}