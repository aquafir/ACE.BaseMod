using ACE.Database.Entity;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Entity.Actions;
using ACE.Server.Factories;
using static ACE.Server.Factories.PlayerFactory;
using Biota = ACE.Entity.Models.Biota;

namespace Ironman.FlagEvents;

[HarmonyPatchCategory(nameof(ForceTemplatePlayer))]
public class ForceTemplatePlayer
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    public static void PreCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
    {
        //Only apply to templated players
        if (characterCreateInfo.TemplateOption == 0)
            return;

        characterCreateInfo.Name = $"{characterCreateInfo.Name}-Im";
        //characterCreateInfo.Name = $"ø{characterCreateInfo.Name}";

        //Randomize heritage / appearance
        //PlayerFactoryEx.RandomizeHeritage(characterCreateInfo);

        //characterCreateInfo.Appearance.ShirtStyle = 0;
        //characterCreateInfo.Appearance.PantsStyle = 0;
        //characterCreateInfo.Appearance.HeadgearStyle = 0;
        //characterCreateInfo.Appearance.FootwearStyle = 0;

        ////Set primary attr
        //var primaryAttr = ThreadSafeRandom.Next(0, 5);
        //characterCreateInfo.CoordinationAbility = 46;
        //characterCreateInfo.EnduranceAbility = 46;
        //characterCreateInfo.FocusAbility = 46;
        //characterCreateInfo.QuicknessAbility = 46;
        //characterCreateInfo.SelfAbility = 46;
        //characterCreateInfo.StrengthAbility = 46;

        //switch (primaryAttr)
        //{
        //    case 0:
        //        characterCreateInfo.CoordinationAbility = 100;
        //        break;
        //    case 1:
        //        characterCreateInfo.EnduranceAbility = 100;
        //        break;
        //    case 2:
        //        characterCreateInfo.FocusAbility = 100;
        //        break;
        //    case 3:
        //        characterCreateInfo.QuicknessAbility = 100;
        //        break;
        //    case 4:
        //        characterCreateInfo.SelfAbility = 100;
        //        break;
        //    case 5:
        //        characterCreateInfo.StrengthAbility = 100;
        //        break;
        //}

        //Reset skills seemed like a pain to do here
        //pendingFinalization.Add(guid.Full);
    }

    //Tracks created but unfinished players
    static readonly HashSet<uint> pendingFinalization = new();
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.DoPlayerEnterWorld), new Type[] { typeof(Session), typeof(Character), typeof(Biota), typeof(PossessedBiotas) })]
    public static void PostDoPlayerEnterWorld(Session session, Character character, Biota playerBiota, PossessedBiotas possessedBiotas)
    {
        //Check for finalizing / grab the player
        if (character.TotalLogins > 1 || session.Player.GetProperty(PropertyString.Template) == "Adventurer")
            return;

        //if (!pendingFinalization.Contains(character.Id))
        //    return;

        var player = PlayerManager.GetOnlinePlayer(character.Id);
        if (player is null)
            return;
        pendingFinalization.Remove(character.Id);

        var actionChain = new ActionChain();
        actionChain.AddDelaySeconds(5);
        actionChain.AddAction(session.Player, () => player.InitializeIronman());
        actionChain.EnqueueChain();
    }

}