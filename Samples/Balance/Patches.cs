namespace Balance;

[HarmonyPatch]
public static class Patches
{
    #region Fields / Props

    #endregion

    #region Start / Stop
    public static void Start()
    {
    }
    public static void Shutdown()
    {
    }
    #endregion

    #region Commands
    //Standalone non-persistent command to adjust level costs
    //[CommandHandler("setmax", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 1, "Set max level.")]
    //public static void HandleMax(Session session, params string[] parameters)
    //{
    //}
    #endregion
}