namespace ChatFilter;

public static class Helper
{
    public static bool IsShadowBanned(this Player player) => player.GetProperty(PatchClass.Settings.ShadowBanPropery) ?? false;
    public static void ShadowBan(this Player player, bool value = true) => player.SetProperty(PatchClass.Settings.ShadowBanPropery, value);
}
