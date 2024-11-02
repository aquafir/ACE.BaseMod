using Recipe = ACE.Database.Models.World.Recipe;

namespace Tower;

[CommandCategory(nameof(Feature.CustomRecipe))]
[HarmonyPatchCategory(nameof(Feature.CustomRecipe))]
public static class CustomRecipePatch
{
    static CustomRecipeSettings Settings => PatchClass.Settings.OnUse;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RecipeManager), nameof(RecipeManager.UseObjectOnTarget), new Type[] { typeof(Player), typeof(WorldObject), typeof(WorldObject), typeof(bool) })]
    public static bool PreUseObjectOnTarget(Player player, WorldObject source, WorldObject target, bool confirmed, ref RecipeManager __instance)
    {
        if (!Settings.Overrides.TryGetValue(source.WeenieClassId, out var customRecipe))
            return true;

        Recipe r;

        //Checks from normal handling
        if (player.IsBusy)
        {
            player.SendUseDoneEvent(WeenieError.YoureTooBusy);
            return false;
        }

        var allowCraftInCombat = PropertyManager.GetBool("allow_combat_mode_crafting").Item;

        if (!allowCraftInCombat && player.CombatMode != CombatMode.NonCombat)
        {
            player.SendUseDoneEvent(WeenieError.YouMustBeInPeaceModeToTrade);
            return false;
        }

        if (source == target)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"The {source.NameWithMaterial} cannot be combined with itself.", ChatMessageType.Craft));
            player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, $"You can't use the {source.NameWithMaterial} on itself."));
            player.SendUseDoneEvent();
            return false;
        }

        var recipe = RecipeManager.GetRecipe(player, source, target);

        //if (recipe == null)
        //{
        //    player.Session.Network.EnqueueSend(new GameMessageSystemChat($"The {source.NameWithMaterial} cannot be used on the {target.NameWithMaterial}.", ChatMessageType.Craft));
        //    player.SendUseDoneEvent();
        //}

        if (Settings.UseRecipeRequirements && recipe is not null)
        {
            // verify requirements
            if (!RecipeManager.VerifyRequirements(recipe, player, source, target))
            {
                player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                return false;
            }

        }

        //Rolling chance / difficulty / requirements could be added to the fake recipe if needed
        //var percentSuccess = GetRecipeChance(player, source, target, recipe);
        //var showDialog = HasDifficulty(recipe) && player.GetCharacterOption(CharacterOption.UseCraftingChanceOfSuccessDialog);

        if (Settings.Animation != MotionCommand.Invalid)
        {
            var actionChain = new ActionChain();
            var nextUseTime = 0.0f;

            player.IsBusy = true;

            if (player.CombatMode != CombatMode.NonCombat)
            {
                // Drop out of combat mode.  This depends on the server property "allow_combat_mode_craft" being True.
                // If not, this action would have aborted due to not being in NonCombat mode.
                var stanceTime = player.SetCombatMode(CombatMode.NonCombat);
                actionChain.AddDelaySeconds(stanceTime);

                nextUseTime += stanceTime;
            }

            var motion = new Motion(player, Settings.Animation);
            var currentStance = player.CurrentMotionState.Stance; // expected to be MotionStance.NonCombat
            var clapTime = !confirmed ? ACE.Server.Physics.Animation.MotionTable.GetAnimationLength(player.MotionTableId, currentStance, Settings.Animation) : 0.0f;

            if (!confirmed)
            {
                actionChain.AddAction(player, () => player.SendMotionAsCommands(Settings.Animation, currentStance));
                actionChain.AddDelaySeconds(clapTime);

                nextUseTime += clapTime;
            }

            //Removed confirmation dialogue
            //if (showDialog && !confirmed)
            //{
            //    actionChain.AddAction(player, () => ShowDialog(player, source, target, recipe, percentSuccess.Value));
            //    actionChain.AddAction(player, () => player.IsBusy = false);
            //}
            //else
            //{
            actionChain.AddAction(player, () => HandleCustomRecipe(player, source, target, recipe, customRecipe));
            actionChain.AddAction(player, () =>
            {
                //if (!showDialog)
                //    player.SendUseDoneEvent();
                player.IsBusy = false;
            });
            //}

            actionChain.EnqueueChain();
            player.NextUseTime = DateTime.UtcNow.AddSeconds(nextUseTime);
        }

        //Return true to execute original
        return false;
    }

    private static void HandleCustomRecipe(Player player, WorldObject source, WorldObject target, Recipe recipe, CustomRecipe customRecipe)
    {
        //Check for item destruction
        if(ThreadSafeRandom.Next(0.0f, 1.0f) < customRecipe.DestroysSource)
        {
            RecipeManager.DestroyItem(player, recipe, source, 1, $"Your {source?.Name} has been destroyed with {customRecipe.DestroysSource:P2}% odds");
        }

        //Roll success
        var success = ThreadSafeRandom.Next(0.0f, 1.0f) < customRecipe.Success;
        if (!success) {
            //?
            player.SendUseDoneEvent(WeenieError.CraftAnimationFailed);
            return;
        }

        //Custom handling
        switch (customRecipe.Handler)
        {
            case CustomUseHandler.RandomColor:
                LootGenerationFactory.RandomizeColorTotallyRandom(target);
                RecipeManager.UpdateObj(player, target);
                break;
            case CustomUseHandler.RandomElement:

                if(target.W_DamageType != 0)
                {
                    DamageType[] types =
                    {
                        DamageType.Acid,
                        DamageType.Cold,
                        DamageType.Electric,
                        DamageType.Fire,

                        DamageType.Bludgeon,
                        DamageType.Pierce,
                        DamageType.Slash,
                    };
                    target.W_DamageType = types.GetRandom();

                    player.SendMessage($"Your {target.Name} now deals {target.W_DamageType}");
                    RecipeManager.UpdateObj(player, target);
                }
                break;
            default:
                break;
        }

        //var modified = CreateDestroyItems(player, recipe, source, target, successChance, success);

        //if (modified != null)
        //{
        //    if (modified.Contains(source.Guid.Full))
        //        UpdateObj(player, source);

        //    if (modified.Contains(target.Guid.Full))
        //        UpdateObj(player, target);
        //}
    }
}
