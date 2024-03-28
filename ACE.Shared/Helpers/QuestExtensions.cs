namespace ACE.Shared.Helpers;

public static class QuestExtensions
{
    /// <summary>
    /// Might need to check logic on this one, but -1 for repeatable?
    /// </summary>
    public static bool HasSolves(this CharacterPropertiesQuestRegistry quest) => quest.NumTimesCompleted != 0;
}
