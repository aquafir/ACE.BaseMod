using ACE.Server.Entity.Actions;

namespace Ironman;

[HarmonyPatch]
public static class FakeIronman
{
    public static void InitializeIronman(this Player player)
    {
        //Randomize appearance / attrs / skills
        player.RollIronmanAppearance();
        player.RollIronmanAttributes();
        player.RollIronmanSkills();

        //Wipe inventory
        player.WipeInventory(true);

        //Wipe spells
        player.RemoveAllSpells();

        //Schedule learning level 1s
        var actionChain = new ActionChain();
        actionChain.AddDelaySeconds(5);
        actionChain.AddAction(player, () =>
        {
            if (PatchClass.Settings.UseSpellList)
            {
                foreach (var spell in PatchClass.Settings.DefaultSpells)
                    player.TryLearnSpell((uint)spell, false);
            }
            else
            {
                player.LearnSpellsInBulk(MagicSchool.CreatureEnchantment, 1);
                player.LearnSpellsInBulk(MagicSchool.ItemEnchantment, 1);
                player.LearnSpellsInBulk(MagicSchool.LifeMagic, 1);
                player.LearnSpellsInBulk(MagicSchool.VoidMagic, 1);
                player.LearnSpellsInBulk(MagicSchool.WarMagic, 1);
            }

            player.UpdateSpellbook();

            player.SendMessage($"Removed all but default spells.");
        });
        actionChain.EnqueueChain();

        //Give items before Ironman applied
        foreach (var skill in player.Skills.Where(x => x.Value.AdvancementClass == SkillAdvancementClass.Specialized))
            player.GiveIronmanItems(skill.Key);

        //Set lives/Ironman props
        player.ApplyIronmanProperty();

        //Setup Hardcore mode
        player.ApplyHardcore();

        //Welcome them
        player.SendMessage(PatchClass.Settings.WelcomeMessage);
        for (var i = 0; i < 20; i++)
            player.PlayParticleEffect(Enum.GetValues<PlayScript>().Random(), player.Guid, i * .02f);
    }

    /// <summary>
    /// Remove all skills then generate up to max credits a plan
    /// </summary>
    public static void RollIronmanSkills(this Player player)
    {
        //Untrain skills
        //player.ResetSkills();
        player.ResetLikeEnlightenment();

        //Local copy of the skill pool
        List<Skill> pool = new(PatchClass.Settings.SecondarySkillPool);
        var log = new StringBuilder();

        //Specialize a primary skill
        var primary = PatchClass.Settings.PrimarySkillPool.Random();
        player.TrainSkill(primary);
        player.SpecializeSkill(primary);
        pool.Remove(primary);
        int remainingCreds = PatchClass.Settings.CreditsToPlanFor + player.AvailableSkillCredits ?? 0;
        log.Append($"\nPrimary skill: {primary}, {remainingCreds} credits left");

        //Train MC if magic was rolled or spec a different skill if not
        HashSet<Skill> trained = new();
        var second = primary == Skill.WarMagic || primary == Skill.VoidMagic || primary == Skill.LifeMagic ?
            Skill.ManaConversion : pool.Where(x => !Settings.AugmentSpecializations.Contains(x)).ToArray().Random();

        player.TrainSkill(second);
        trained.Add(second);

        if (second != Skill.ManaConversion)
        {
            player.SpecializeSkill(second);
            pool.Remove(second);  //No spec'ing after initial
        }
        remainingCreds = PatchClass.Settings.CreditsToPlanFor + player.AvailableSkillCredits ?? 0;
        log.Append($"\nSecondary skill: {second}, {remainingCreds} credits left");

        //Train random until a failure
        //int numSpec = 1;
        var plan = new StringBuilder();

        while (true)
        {
            //Get a random skill
            var skill = pool.Random();

            //Skip attempts to specialize past the allowed amount
            bool spec = trained.Contains(skill);
            //if (spec && numSpec >= PatchClass.Settings.MaxSpecializedSkills)
            //    continue;
            if (spec)
                continue;

            //Get the skill credit cost
            if (!DatManager.PortalDat.SkillTable.SkillBaseHash.TryGetValue((uint)skill, out var skillBase))
            {
                ModManager.Log($"Error looking up {skill} skill data.", ModManager.LogLevel.Error);
                break;
            }
            var cost = spec ? skillBase.UpgradeCostFromTrainedToSpecialized : skillBase.TrainedCost;

            //Check if done
            if (remainingCreds < cost)
                break;

            //If credits are available learn the skill
            if (player.AvailableSkillCredits.Value >= remainingCreds)
            {
                if (spec)
                    player.SpecializeSkill(skill);
                else
                    player.TrainSkill(skill);
            }
            //Otherwise add the skill to the plan
            else
            {
                plan.Append($"{skill};");

                //And update the pool
                if (spec)
                    pool.Remove(skill);
                else
                    trained.Add(skill);
            }

            //Update the pool and credits
            remainingCreds -= cost;

            //Remove from the pool and increase the tally
            pool.Remove(skill);
            //if (spec)
            //{
            //    pool.Remove(skill);
            //    //numSpec++;
            //}
            //else
            //    trained.Add(skill);

            log.Append($"\n{(spec ? "Specialized" : "Trained")} {skill} for {cost}, {remainingCreds} credits left");
        }

        //Set the plan / output what's done
        player.SetProperty(FakeString.IronmanPlan, plan.ToString());
        player.SetProperty(FakeString.IronmanFullPlan, log.ToString());
        ModManager.Log(log.ToString());

        player.SendUpdatedSkills();

        //Add items
        player.GiveIronmanItems(primary);
    }

    public static void GiveIronmanItems(this Player player, Skill primary)
    {
        if (!PatchClass.Settings.SkillItems.TryGetValue(primary, out var items) || items is null)
            return;

        player.SetProperty(FakeBool.Ironman, false);
        foreach (var item in items)
            player.TryCreateItems(item);

        player.SetProperty(FakeBool.Ironman, true);
        foreach (var item in player.Inventory.Values)
            item.SetProperty(FakeBool.Ironman, true);

        //foreach(var item in player.EquippedObjects.Values)
        //    item.SetProperty(FakeBool.Ironman, true);
    }

    /// <summary>
    /// Roll a random primary attribute
    /// </summary>
    /// <param name="player"></param>
    public static void RollIronmanAttributes(this Player player)
    {
        if (player is null)
            return;

        PropertyAttribute primary = (PropertyAttribute)ThreadSafeRandom.Next(1, 6);
        foreach (var attr in Enum.GetValues<PropertyAttribute>())
        {
            if (attr == PropertyAttribute.Undef) continue;

            var pAttr = player.Attributes[attr];
            pAttr.StartingValue = attr == primary ? 100u : 46u;

            player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, pAttr));
        }
    }

    /// <summary>
    /// Randomize heritage/gender/appearance
    /// </summary>
    /// <param name="player"></param>
    public static void RollIronmanAppearance(this Player player)
    {
        if (player is null)
            return;

        player.RandomizeAppearance();
        player.EnqueueBroadcastUpdateObject();
    }

    /// <summary>
    /// Add Ironman property and related mode setup
    /// </summary>
    /// <param name="player"></param>
    public static void ApplyIronmanProperty(this Player player)
    {
        if (player is null)
            return;

        if (player.GetProperty(FakeBool.Ironman) == true)
        {
            player.SendMessage($"You are already an Ironman.");
            return;
        }

        player.SetProperty(FakeBool.Ironman, true);
        player.RadarColor = RadarColor.Sentinel;
        player.SendMessage($"You're now participating in Ironman.  Stay safe!  Unless you rolled trash");
    }

    /// <summary>
    /// Add Hardcore property and related mode setup
    /// </summary>
    public static void ApplyHardcore(this Player player)
    {
        if (player is null)
            return;

        if (!PatchClass.Settings.Restrictions.Contains(nameof(Hardcore)) || player.GetProperty(FakeBool.Hardcore) == true)
        {
            player.SendMessage($"You are already Hardcore.");
            return;
        }

        player.SetProperty(FakeInt.HardcoreLives, PatchClass.Settings.HardcoreStartingLives);
        player.SetProperty(FakeFloat.TimestampLastPlayerDeath, Time.GetUnixTime());
        player.SetProperty(FakeBool.Hardcore, true);
        player.SendMessage($"\nYou have {PatchClass.Settings.HardcoreStartingLives} remaining and {PatchClass.Settings.HardcoreSecondsBetweenDeathAllowed} seconds between deaths.");
    }

    /// <summary>
    /// Remove Ironman status
    /// </summary>
    /// <param name="player"></param>
    public static void RemoveIronman(this Player player)
    {
        if (player is null)
            return;

        if (player.GetProperty(FakeBool.Ironman) == true)
        {
            player.SendMessage($"You are not an Ironman.");
            return;
        }

        if (player is null)
            return;

        player.SetProperty(FakeBool.Ironman, false);
        player.SetProperty(FakeBool.Hardcore, false);
        player.RadarColor = RadarColor.Default;
        player.SendMessage("You're no longer participating in Ironman");
    }

    /// <summary>
    /// Flag all items in Container for Ironman
    /// </summary>
    public static void SetClaimedBy(this Container container, Player player)
    {
        foreach (var item in container.Inventory.Values)
            item.SetProperty(FakeBool.Ironman, true);

        //Debug
        var sb = new StringBuilder($"\n{player.Name} is claiming {container.Name}");
        foreach (var item in container.Inventory.Values)
            sb.Append($"\n  {item.Name}");

        player.SendMessage(sb.ToString());
    }
}