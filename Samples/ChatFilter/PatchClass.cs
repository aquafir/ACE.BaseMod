
namespace ChatFilter;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;
        PatchCategories();
        SetupFilter();
    }

    #region Setup
    private void PatchCategories()
    {
        if (Settings.FilterChat)
            ModC.Harmony.PatchCategory(Settings.ChatCategory);

        if (Settings.FilterTells)
            ModC.Harmony.PatchCategory(Settings.TellCategory);
    }

    static ProfanityFilter.ProfanityFilter filter = new();
    private void SetupFilter()
    {
        //Create default or empty list
        filter = Settings.UseDefaultList ? new ProfanityFilter.ProfanityFilter() : new ProfanityFilter.ProfanityFilter(new string[] { });

        //Load blacklist
        var watch = Stopwatch.StartNew();
        if (File.Exists(Settings.BlackList))
        {
            var list = File.ReadAllLines(Settings.BlackList);
            filter.AddProfanity(list);

            ModManager.Log($"ChatFilter: Blacklisted {list.Length} words after {watch.ElapsedMilliseconds} ms");
            watch.Restart();

        }
        if (File.Exists(Settings.WhiteList))
        {
            var list = File.ReadAllLines(Settings.WhiteList);
            filter.RemoveProfanity(list);

            ModManager.Log($"ChatFilter: Whitelisted {list.Length} words after {watch.ElapsedMilliseconds} ms");
            watch.Stop();
        }
    }
    #endregion

    #region Commands
    // unsban [name]
    [CommandHandler("unsban", AccessLevel.Sentinel, CommandHandlerFlag.None, 1, "Removes shadow ban from a player.")]
    public static void HandleShadowbanRemove(Session session, params string[] parameters)
    {
        var name = parameters[0];
        if (PlayerManager.FindByName(name) is not Player player)
        {
            ModManager.Log($"Unable to remove shadow ban from offline player: {name}");
            return;
        }

        player.SetShadowBanned(false);
        player.SendMessage("Your are no longer shadow banned.");
    }
    #endregion

    public static bool TryHandleToxicity(ref string message, Player player, ChatSource source, string? target = null)
    {
        var hasProfanity = filter.ContainsProfanity(message);

        //Check shadowban before anything.  Skips recipient but sends user?
        if (Settings.ShadowBan)
        {
            if (!player.IsShadowBanned())
                player.SetShadowBanned(true);

            //Todo: If a player was shadow banned ban ALL chat or just filtered chat?
            if (player.IsShadowBanned())
            {
                switch (source)
                {
                    case ChatSource.Chat:
                        player.FakeChat(message);
                        break;
                    case ChatSource.Tell:
                        player.FakeTell(message, target);
                        break;
                }

                return true;
            }
        }

        //Unsure about a short circuit.  Allow non-profane messages, or block all?
        if (!hasProfanity)
            return false;

        player.IncreaseChatInfractionCount();

        //Replace words but don't stop
        if (Settings.CensorText)
        {
            message = filter.CensorString(message);
            return false;
        }

        if (Settings.GagPlayer)
            player.GagPlayer();

        if (Settings.BanAccount)
            player.BanPlayerAccount();

        return true;
    }
}
public enum ChatSource
{
    Chat,
    Tell
}
