namespace Tower;

public static class MeleeMagicExtensions
{
    static MeleeMagicSettings Settings => PatchClass.Settings.MeleeMagic;

    public static bool TryGetMeleeMagicSpell(this Player player, DamageEvent attack, out SpellId spell)//, out List<MeleeMagicPool> pools)
    {
        spell = SpellId.Undef;

        if (!player.TryGetMeleeMagicPools(attack, out var pools))
            return false;

        //Todo: decide on supporting multiple spells / different policies for how many spells are taken per limiting Skill
        //HashSet<Skill> skillsUsed = new();

        var power = player.GetPowerAccuracyBar();

        //Look through the most restrictive first
        foreach (var pool in pools.OrderByDescending(x => x.MinimumSlider))
        {
            //See if there are no more eligible pools
            if (pool.MinimumSlider > power)
                continue;
            //player.SendMessage($"Failed to find slider pool below {power} - {pool.LimitingSkill} > {pool.MinimumSlider}");

            //Fetch the player skill
            if (!player.Skills.TryGetValue(pool.LimitingSkill, out var skill))
                continue;

            //Require trained
            if (skill.AdvancementClass < SkillAdvancementClass.Trained)
                continue;


            //player.SendMessage($"Check {pool.LimitingSkill} > {pool.MinimumSlider} - {power}");

            //Look for most restricted spell
            for (var i = pool.Spells.Count - 1; i >= 0; --i)
            {
                if (skill.Base >= pool.Spells.Keys[i])
                {
                    spell = pool.Spells.Values[i];

                    //player.SendMessage($"Selected {spell} @ {pool.Spells.Keys[i]} >= {skill.Base}");
                    return true;
                }
            }
        }

        return false;
    }

    public static bool TryGetMeleeMagicPools(this Player player, DamageEvent attack, out List<MeleeMagicPool> pools)
    {
        pools = null;

        if (!player.TryGetMeleeMagicGroup(out var group))
            return false;

        //Get pool candidates
        return group.Pools.TryGetValue(attack.AttackHeight, out pools);
    }

    public static bool TryGetMeleeMagicGroup(this Player player, out MeleeMagicGroup group)
    {
        group = null;


        //Check for unarmed or a valid weapon
        var weapon = player.GetEquippedWeapon();

        if (weapon is not null && weapon.GetProperty(FakeDID.MeleeMagicGroup) is null)
            return false;

        //Get the ID (or default)
        var id = weapon is null ? Settings.DefaultGroup : weapon.GetProperty(FakeDID.MeleeMagicGroup) ?? Settings.DefaultGroup;

        if (Settings.MeleeMagicGroups.TryGetValue(id, out group))
            return true;

        return false;
    }
}