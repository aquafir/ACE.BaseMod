namespace ChatFilter;

[HarmonyPatchCategory(Settings.ChatCategory)]
internal static class OnTalk
{
    //White text
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionTalk), new Type[] { typeof(string) })]
    public static bool PreHandleActionTalk(ref string message, ref Player __instance)
    {
        if (PatchClass.Settings.FilterChat)
        {
            if (PatchClass.TryHandleToxicity(ref message, __instance, ChatSource.Chat))
                return false;
        }

        return true;
    }
}