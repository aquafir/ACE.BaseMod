namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(HealingDifficulty))]
    public class HealingDifficulty : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "2x*1.1c";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,      // Missing health
            //["p"] = MType.Float,    // Health percent -- 5-15-2023 method changed to remove vital
            ["c"] = MType.Int,      // 0/1 out/in combat mode
                                    // Difficulty for skill check
        };

        static Func<int, int, int> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(HealingDifficulty));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Healer), nameof(Healer.DoSkillCheck), new Type[] { typeof(Player), typeof(Player), typeof(uint), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
        public static bool PreDoSkillCheck(Player healer, Player target, uint missingVital, int difficulty, ref Healer __instance, ref bool __result)
        {
            // skill check:
            // (healing skill + healing kit boost) * trainedMod
            // vs. damage * 2 * combatMod
            var healingSkill = healer.GetCreatureSkill(Skill.Healing);
            var trainedMod = healingSkill.AdvancementClass == SkillAdvancementClass.Specialized ? 1.5f : 1.1f;

            //var combatMod = healer.CombatMode == CombatMode.NonCombat ? 1.0f : 1.1f;
            var combatMod = healer.CombatMode == CombatMode.NonCombat ? 0 : 1;

            //var missingHealth = (int)(vital.MaxValue - vital.Current);
            //var healthPercent = 1 - ((float)missingHealth / vital.MaxValue);

            var effectiveSkill = (int)Math.Round((healingSkill.Current + __instance.BoostValue) * trainedMod);
            difficulty = func((int)missingVital, combatMod);
            //(int)Math.Round(missingHealth * 2 * combatMod);

            var skillCheck = SkillCheck.GetSkillChance(effectiveSkill, difficulty);
            __result = skillCheck > ThreadSafeRandom.Next(0.0f, 1.0f);

            return false;
        }

        #endregion

    }
}