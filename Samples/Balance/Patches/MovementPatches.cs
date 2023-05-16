using ACE.Server.Network.Structure;

namespace Balance.Patches;

[HarmonyPatch]
public class MovementPatches : AngouriMathPatch
{
    #region Fields / Props
    //s = current strength
    public string JumpFormula { get; set; } = "3";   //Multiplier of velocity
    //r = runrate, q = current quick
    public string RunFormula { get; set; } = "1.2 * r * q/100"; //Sets runrate


    static Func<int, float>? jumpFunc;         //s = current strength
    static Func<float, int, float>? runFunc;   //r = runrate, q = current quick
    #endregion

    #region Start / Stop
    public static void Start()
    {
        try
        {
            jumpFunc = PatchClass.Settings.JumpFormula.CompileFriendly().Compile<int, float>("s");
        }
        catch (Exception e)
        {
            ModManager.Log("Failed to parse equation: " + PatchClass.Settings.JumpFormula, ModManager.LogLevel.Error);
        }
        try
        {
            runFunc = PatchClass.Settings.RunFormula.CompileFriendly().Compile<float, int, float>("r", "q");
        }
        catch (Exception e)
        {
            ModManager.Log("Failed to parse equation: " + PatchClass.Settings.RunFormula, ModManager.LogLevel.Error);
        }
    }
    public static void Shutdown()
    {
    }
    #endregion

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GetRunRate), new Type[] { })]
    public static void PostGetRunRate(ref Creature __instance, ref float __result)
    {
        if (__instance is Player player && runFunc is not null)
        {
            __result = runFunc(__result, (int)__instance.Quickness.Current);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionJump), new Type[] { typeof(JumpPack) })]
    public static bool PreHandleActionJump(JumpPack jump, ref Player __instance)
    {
        if (jumpFunc is not null)
        {
            jump.Velocity *= jumpFunc((int)__instance.Strength.Current);
        }

        //Return true to execute original
        return true;
    }
}