using ACE.Database.Entity;
using ACE.Server.Entity.Actions;
using static ACE.Server.Factories.PlayerFactory;
using Biota = ACE.Entity.Models.Biota;

namespace Ironman.FlagEvents;

[HarmonyPatchCategory(nameof(ForceTemplatePlayer))]
public class ForceTemplatePlayer
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    public static void PreCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
    {
        //Only apply to templated players
        if (characterCreateInfo.TemplateOption == 0)
            return;

        characterCreateInfo.Name = $"{characterCreateInfo.Name}-Im";
        __result = CreateResult.Success;
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
        actionChain.AddAction(session.Player, () => player.Teleport(player.Location));
        actionChain.EnqueueChain();
    }

}