namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeKillTask))]
[HarmonyPatchCategory(nameof(Feature.FakeKillTask))]
internal class FakeKillTask
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnDeath_HandleKillTask), new Type[] { typeof(string) })]
    public static bool PreOnDeath_HandleKillTask(string killQuest, ref Creature __instance)
    {
        var summon_credit_cap = (int)PropertyManager.GetLong("summoning_killtask_multicredit_cap").Item - 1;
        var playerCredits = new Dictionary<ObjectGuid, int>();
        var summonCredits = new Dictionary<ObjectGuid, int>();

        // this option isn't really needed anymore, but keeping it around for compatibility
        // it is now synonymous with summoning_killtask_multicredit_cap <= 1
        //if (!PropertyManager.GetBool("allow_summoning_killtask_multicredit").Item)
        //    summon_credit_cap = 0;

        //Debugger.Break();
        //For each who got damage in
        foreach (var kvp in __instance.DamageHistory.TotalDamage)
        {
            if (kvp.Value.TotalDamage <= 0)
                continue;

            var damager = kvp.Value.TryGetAttacker();
            var combatPet = false;
            var playerDamager = damager as Player;

            if (playerDamager == null && kvp.Value.PetOwner != null)
            {
                playerDamager = kvp.Value.TryGetPetOwner();
                combatPet = true;
            }

            if (playerDamager == null)
                continue;

            var killTaskCredits = combatPet ? summonCredits : playerCredits;

            var cap = combatPet ? summon_credit_cap : 1;

            if (cap <= 0)
            {
                // handle special case: use playerCredits
                killTaskCredits = playerCredits;
                cap = 1;
            }

            if (playerDamager.QuestManager.HasQuest(killQuest))
            {
                var equipBonus = playerDamager.GetCachedFake(FakeInt.ItemKillTaskBonus);
                for (var i = 0; i <= equipBonus; i++)
                    __instance.TryHandleKillTask(playerDamager, killQuest, killTaskCredits, cap + equipBonus);
            }
            // check option that requires killer to have killtask to pass to fellows
            else if (!PropertyManager.GetBool("fellow_kt_killer").Item)
            {
                continue;
            }

            if (playerDamager.Fellowship == null)
                continue;

            // share with fellows in kill task range
            var fellows = playerDamager.Fellowship.WithinRange(playerDamager);

            foreach (var fellow in fellows)
            {
                if (fellow.QuestManager.HasQuest(killQuest))
                {
                    //Give bonus to fellow?
                    var equipBonus = fellow.GetCachedFake(FakeInt.ItemKillTaskBonus);
                    for (var i = 0; i <= equipBonus; i++)
                        __instance.TryHandleKillTask(fellow, killQuest, killTaskCredits, cap + equipBonus);
                }
            }
        }

        return false;
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Creature), nameof(Creature.TryHandleKillTask), new Type[] { typeof(Player), typeof(string), typeof(Dictionary<ObjectGuid, int>), typeof(int) })]
    //public static bool PreTryHandleKillTask(Player player, string killTask, Dictionary<ObjectGuid, int> killTaskCredits, ref int cap, ref Creature __instance, ref bool __result)
    //{
    //    //Rewrite.  The ACE approach seems weird and wasteful but might just be me?
    //    if (killTaskCredits.TryGetValue(player.Guid, out var currentCredits))
    //    {
    //        if (currentCredits >= cap)
    //            __result = false;

    //        //killTaskCredits[player.Guid]++;
    //        killTaskCredits[player.Guid] += player.GetCachedFake(FakeInt.ItemKillTaskBonus);
    //    }
    //    else
    //    {
    //        killTaskCredits[player.Guid] = 1;
    //        //player
    //    }

    //    player.QuestManager.HandleKillTask(killTask, __instance);

    //    __result = true;

    //    return false;
    //}
}
