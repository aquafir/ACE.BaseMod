
namespace EasyEnlightenment;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    private const int ENLIGHTENMENT_WCID = 53412;

    public override async Task OnStartSuccess()
    {
        Settings = SettingsContainer.Settings;
    }

    [CommandHandler("newlum", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleMeta(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (player.GetProperty(FakeBool.UsingNewLuminance) ?? true)
        {
            player.SendMessage($"You're already using the new enlightenment system.");
            return;
        }

        var oldEnlightenment = player.Enlightenment;
        player.Enlightenment = 0;

        for (var i = 0; i < oldEnlightenment; i++)
            Enlightenment.AddPerks(null, player);

        player.SendMessage($"You're on the new system with {oldEnlightenment} enlightenments.");
        player.SetProperty(FakeBool.UsingNewLuminance, true);
    }

    [CommandHandler("fixee", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleFix(Session session, params string[] parameters)
    {
        ApplyBonuses(session.Player);
    }

    static void ApplyBonuses(Player player)
    {
        //PRESUMES NO OTHER BONUSES TO SET PROPS
        var e = player.Enlightenment;

        //Custom props
        foreach (var prop in Settings.IntAugments)
        {
            var current = player.GetProperty(prop.Key) ?? 0;
            //var value = prop.Key.ToString().StartsWith("Lum") ? player.Enlightenment * prop.Value : current + prop.Value;
            var value = player.Enlightenment * prop.Value;
            player.UpdateProperty(player, prop.Key, value, true);
            player.SendMessage($"You've been awarded {value} {prop.Key}");
        }
        foreach (var prop in Settings.FloatAugments)
        {
            var current = player.GetProperty(prop.Key) ?? 0;
            //var value = prop.Key.ToString().StartsWith("Lum") ? player.Enlightenment * prop.Value : current + prop.Value;
            var value = player.Enlightenment * prop.Value;
            player.UpdateProperty(player, prop.Key, value, true);
            player.SendMessage($"You've been awarded {value} {prop.Key}");
        }

        //Bonuses enabled with Expansion
        foreach (var prop in Settings.SkillAugments)
        {
            var value = prop.Value * player.Enlightenment;
            player.SetBonus(prop.Key, value);
            player.SendUpdated(prop.Key);
            player.SendMessage($"You've been awarded {value} {prop.Key}");
        }
        foreach (var prop in Settings.AttributeAugments)
        {
            var value = prop.Value * player.Enlightenment;
            player.SetBonus(prop.Key, value);
            player.SendUpdated(prop.Key);
            player.SendMessage($"You've been awarded {value} {prop.Key}");
        }
        foreach (var prop in Settings.VitalAugments)
        {
            var value = prop.Value * player.Enlightenment;
            player.SetBonus(prop.Key, value);
            player.SendUpdated(prop.Key);
            player.SendMessage($"You've been awarded {value} {prop.Key}");
        }
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(Enlightenment), nameof(Enlightenment.RemoveAbility), new Type[] { typeof(Player) })]
    public static bool PreRemoveAbility(Player player, ref Enlightenment __instance)
    {
        if (Settings.RemoveSociety)
            Enlightenment.RemoveSociety(player);
        if (Settings.RemoveLuminance)
            Enlightenment.RemoveLuminance(player);
        if (Settings.RemoveAetheria)
            Enlightenment.RemoveAetheria(player);
        if (Settings.RemoveAttributes)
            Enlightenment.RemoveAttributes(player);
        if (Settings.RemoveSkills)
            Enlightenment.RemoveSkills(player);
        if (Settings.RemoveLevel)
            Enlightenment.RemoveLevel(player);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CheckUseRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckUseRequirements(WorldObject activator, ref WorldObject __instance, ref ActivationResult __result)
    {
        //Handle activator custom
        if (__instance.WeenieClassId == ENLIGHTENMENT_WCID)
        {
            if (activator is not Player player)
                __result = new ActivationResult(true);
            //else if (player.Level < Settings.LevelReq)
            //{
            //    __result = new ActivationResult(new GameEventCommunicationTransientString(player.Session, "You do not meet the requiremenets for Enlightenment."));

            //    player.SendMessage($"Requirements:\n" +
            //        $"Over level {Settings.LevelReq}\n" +
            //        $"Under {Settings.MaxEnlightenments} enlightenments\n" +
            //        $"Require Society Master? {Settings.RequireSocietyMaster}\n" +
            //        $"All luminance? {Settings.RequireAllLuminanceAuras}");
            //}
            else
            {
                __result = new ActivationResult(new GameEventCommunicationTransientString(player.Session, $"Sneaking in the 'ol enlightenment here."));
                Enlightenment.HandleEnlightenment(activator, player);
            }
            return false;
        }

        //Return true to execute original
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Enlightenment), nameof(Enlightenment.AddPerks), new Type[] { typeof(WorldObject), typeof(Player) })]
    public static bool PreAddPerks(WorldObject npc, Player player, ref Enlightenment __instance)
    {
        player.Enlightenment += 1;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.Enlightenment, player.Enlightenment));

        if (Settings.SkipNormalBroadcast)
        {
            player.SendMessage("You have become enlightened and view the world with new eyes.", ChatMessageType.Broadcast);
            player.SendMessage("Your available skill credits have been adjusted.", ChatMessageType.Broadcast);
            player.SendMessage("You have risen to a higher tier of enlightenment!", ChatMessageType.Broadcast);
        }

        var lvl = "";

        // add title
        switch (player.Enlightenment)
        {
            case 1:
                player.AddTitle(CharacterTitle.Awakened);
                lvl = "1st";
                break;
            case 2:
                player.AddTitle(CharacterTitle.Enlightened);
                lvl = "2nd";
                break;
            case 3:
                player.AddTitle(CharacterTitle.Illuminated);
                lvl = "3rd";
                break;
            case 4:
                player.AddTitle(CharacterTitle.Transcended);
                lvl = "4th";
                break;
            case 5:
                player.AddTitle(CharacterTitle.CosmicConscious);
                lvl = "5th";
                break;
            default:
                lvl = $"{player.Enlightenment}th";
                break;
        }

        if (!Settings.SkipResetCertificate)
            player.GiveFromEmote(npc, Enlightenment.AttributeResetCertificate, 1);

        var msg = $"{player.Name} has achieved the {lvl} level of Enlightenment!";
        PlayerManager.BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));
        PlayerManager.LogBroadcastChat(Channel.AllBroadcast, null, msg);


        //Custom props
        ApplyBonuses(player);

        //Credits
        var creditsOwed = player.Enlightenment / Settings.SkillCreditInterval * Settings.SkillCreditAmount;
        player.AddSkillCredits(player.Enlightenment / Settings.SkillCreditInterval * Settings.SkillCreditAmount);
        player.SendMessage($"You've been awarded {creditsOwed} skill credits.");

        //Max lum
        if (player.MaximumLuminance is not null)
        {
            var maxLuminance = Settings.MaxLumBase + Settings.MaxLumPerEnlightenment * player.Enlightenment;
            player.MaximumLuminance = maxLuminance;
            player.SendMessage($"Your luminance is now {maxLuminance}");

            player.UpdateProperty(player, PropertyInt64.MaximumLuminance, maxLuminance);
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Enlightenment), nameof(Enlightenment.VerifyRequirements), new Type[] { typeof(Player) })]
    public static bool PreVerifyRequirements(Player player, ref Enlightenment __instance, ref bool __result)
    {
        __result = true;

        if (player.Level < Settings.LevelReq)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You must be level 275 for enlightenment.", ChatMessageType.Broadcast));
            __result = false;
        }
        else if (Settings.RequireAllLuminanceAuras && !VerifyLumAugs(player))
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You must have all luminance auras for enlightenment.", ChatMessageType.Broadcast));
            __result = false;
        }

        else if (Settings.RequireSocietyMaster && !Enlightenment.VerifySocietyMaster(player))
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You must be a Master of one of the Societies of Dereth for enlightenment.", ChatMessageType.Broadcast));
            __result = false;
        }

        else if (player.GetFreeInventorySlots() < 25)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You must have at least 25 free inventory slots in your main pack for enlightenment.", ChatMessageType.Broadcast));
            __result = false;
        }

        else if (player.Enlightenment >= Settings.MaxEnlightenments)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You have already reached the maximum enlightenment level!", ChatMessageType.Broadcast));
            __result = false;
        }

        return false;
    }

    //Override lum check for minimums
    //Todo: improve this
    public static bool VerifyLumAugs(Player player)//, ref Enlightenment __instance, ref bool __result)
    {
        //Have all luminance auras(crafting aura included) except the 2 skill credit auras. (20 million total luminance)
        var lumAugCredits = 0;

        lumAugCredits += player.LumAugAllSkills;
        lumAugCredits += player.LumAugSurgeChanceRating;
        lumAugCredits += player.LumAugCritDamageRating;
        lumAugCredits += player.LumAugCritReductionRating;
        lumAugCredits += player.LumAugDamageRating;
        lumAugCredits += player.LumAugDamageReductionRating;
        lumAugCredits += player.LumAugItemManaUsage;
        lumAugCredits += player.LumAugItemManaGain;
        lumAugCredits += player.LumAugHealingRating;
        lumAugCredits += player.LumAugSkilledCraft;
        lumAugCredits += player.LumAugSkilledSpec;

        return lumAugCredits >= Settings.BaseLumAugmentationsRequired + player.Enlightenment * Settings.LumAugmentationsRequiredPerEnlightenment;
    }
}

