using ACE.Server.WorldObjects;
using static ACE.Server.WorldObjects.Player;
using Recipe = ACE.Database.Models.World.Recipe;

namespace Tower;

[CommandCategory(nameof(Feature.CustomRecipe))]
[HarmonyPatchCategory(nameof(Feature.CustomRecipe))]
public static class CustomRecipePatch
{
    static CustomRecipeSettings Settings => PatchClass.Settings.OnUse;

    //Patches GameAction handler
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionUseWithTarget), new Type[] { typeof(ObjectGuid), typeof(ObjectGuid) })]
    public static bool PreHandleActionUseWithTarget(ObjectGuid sourceObjectGuid, ObjectGuid targetObjectGuid, ref Player __instance)
    {
        //Find items
        var source = __instance.FindObject(sourceObjectGuid, SearchLocations.MyInventory | SearchLocations.MyEquippedItems, out _, out _, out var sourceItemIsEquipped);
#if REALM
var target = __instance.FindObject(targetObjectGuid, SearchLocations.MyInventory | SearchLocations.MyEquippedItems | SearchLocations.Landblock);
#else
        var target = __instance.FindObject(targetObjectGuid.Full, SearchLocations.MyInventory | SearchLocations.MyEquippedItems | SearchLocations.Landblock);
# endif

        if (source is null || target is null)
            return true;

        if (!Settings.Overrides2.TryGetValue(source.WeenieClassId, out var customRecipe))
            return true;

        Handle(__instance, source, target, false, customRecipe);
        __instance.SendUseDoneEvent();

        //Return true to execute original
        return false;
    }

    public static void Handle(Player player, WorldObject source, WorldObject target, bool confirmed, CustomRecipeEx customRecipe)
    {
        //Todo: figure out why compile-time collection defaults fail
        customRecipe.Animations ??= [MotionCommand.ClapHands];

        if (customRecipe.NotBusy && player.IsBusy)
        {
            player.SendUseDoneEvent(WeenieError.YoureTooBusy);
            return;
        }

        var changeStance = PropertyManager.GetBool("allow_combat_mode_crafting").Item;
        if (!changeStance && player.CombatMode != customRecipe.Stance)
        {
            player.SendUseDoneEvent(WeenieError.YouMustBeInPeaceModeToTrade);
            return;
        }

        if (!customRecipe.UseOnSelf && source == target)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"The {source.NameWithMaterial} cannot be combined with itself.", ChatMessageType.Craft));
            player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, $"You can't use the {source.NameWithMaterial} on itself."));
            player.SendUseDoneEvent();
            return;
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
                return;
            }

        }

        //Rolling chance / difficulty / requirements could be added to the fake recipe if needed

        var actionChain = new ActionChain();
        var nextUseTime = 0.0f;

        player.IsBusy = true;

        //Queue each motion
        foreach (var animation in customRecipe.Animations)
        {
            if (animation != MotionCommand.Invalid)
            {
                if (player.CombatMode != CombatMode.NonCombat)
                {
                    // Switch mode
                    var stanceTime = player.SetCombatMode(customRecipe.Stance);
                    actionChain.AddDelaySeconds(stanceTime);
                    nextUseTime += stanceTime;
                }

                var motion = new Motion(player, animation);
                var currentStance = player.CurrentMotionState.Stance; // expected to be MotionStance.NonCombat
                var animTime = !confirmed ? ACE.Server.Physics.Animation.MotionTable.GetAnimationLength(player.MotionTableId, currentStance, animation) : 0.0f;

                if (!confirmed)
                {
                    actionChain.AddAction(player, () => player.SendMotionAsCommands(animation, currentStance));
                    actionChain.AddDelaySeconds(animTime);

                    nextUseTime += animTime;
                }
            }
        }

        actionChain.AddAction(player, () => HandleCustomRecipe(player, source, target, recipe, customRecipe));
        actionChain.AddAction(player, () =>
        {
            player.SendUseDoneEvent();
            player.IsBusy = false;
        });
        actionChain.EnqueueChain();
        player.NextUseTime = DateTime.UtcNow.AddSeconds(nextUseTime);

        //HandleCustomRecipe(player, source, target, recipe, customRecipe);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RecipeManager), nameof(RecipeManager.UseObjectOnTarget), new Type[] { typeof(Player), typeof(WorldObject), typeof(WorldObject), typeof(bool) })]
    public static bool PreUseObjectOnTarget(Player player, WorldObject source, WorldObject target, bool confirmed)
    {
        if (!Settings.Overrides.TryGetValue(source.WeenieClassId, out var customRecipe))
            return true;

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
            var animTime = !confirmed ? ACE.Server.Physics.Animation.MotionTable.GetAnimationLength(player.MotionTableId, currentStance, Settings.Animation) : 0.0f;

            if (!confirmed)
            {
                actionChain.AddAction(player, () => player.SendMotionAsCommands(Settings.Animation, currentStance));
                actionChain.AddDelaySeconds(animTime);

                nextUseTime += animTime;
            }

            actionChain.AddAction(player, () => HandleCustomRecipe(player, source, target, recipe, customRecipe));
            actionChain.AddAction(player, () =>
            {
                player.SendUseDoneEvent();
                player.IsBusy = false;
            });
            //}

            actionChain.EnqueueChain();
            player.NextUseTime = DateTime.UtcNow.AddSeconds(nextUseTime);
        }

        //Return true to execute original
        return false;
    }


    private static void HandleCustomRecipe(Player player, WorldObject source, WorldObject target, Recipe recipe, CustomRecipeEx customRecipe)
        => HandleCustomRecipe(player, source, target, recipe, customRecipe.DestroysSource, customRecipe.Success, customRecipe.Handler);
    private static void HandleCustomRecipe(Player player, WorldObject source, WorldObject target, Recipe recipe, CustomRecipe customRecipe)
        => HandleCustomRecipe(player, source, target, recipe, customRecipe.DestroysSource, customRecipe.Success, customRecipe.Handler);

    private static void HandleCustomRecipe(Player player, WorldObject source, WorldObject target, Recipe recipe, double destroysSource, double successOdds, CustomUseHandler handler)
    {
        //Check for item destruction
        if (ThreadSafeRandom.Next(0.0f, 1.0f) < destroysSource)
        {
            RecipeManager.DestroyItem(player, recipe, source, 1, $"Your {source?.Name} has been destroyed with {destroysSource:P2}% odds");
        }

        //Roll success
        var success = ThreadSafeRandom.Next(0.0f, 1.0f) < successOdds;
        if (!success)
        {
            player.SendUseDoneEvent(WeenieError.CraftAnimationFailed);
            return;
        }

        //Custom handling
        switch (handler)
        {
            case CustomUseHandler.RandomColor:
                LootGenerationFactory.RandomizeColorTotallyRandom(target);
                RecipeManager.UpdateObj(player, target);
                break;
            case CustomUseHandler.RandomElement:
                if (target.W_DamageType != 0)
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
            case CustomUseHandler.Launch:
                if (target is Creature creature)
                {
                    foreach (var item in creature.EquippedObjects.Values)
                    {
                        LootGenerationFactory.RandomizeColorTotallyRandom(item);
                        //RecipeManager.UpdateObj(player, target);
                    }

                    player.TryCastSpell_WithRedirects(new Spell(SpellId.FlameArc1), creature, player.GetEquippedWand());
                }
                break;
            default:
                break;
        }
    }
}
