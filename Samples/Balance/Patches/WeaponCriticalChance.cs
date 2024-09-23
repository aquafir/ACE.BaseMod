namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(WeaponCriticalChance))]
    public class WeaponCriticalChance : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "x + .01r";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Float,    // Larger of the two bonuses, before rating is added
            ["f"] = MType.Float,    // Weapon frequency or .1 if null
            ["c"] = MType.Float,    // Imbue modifier or 0 if not imbued
            ["r"] = MType.Int,      // Crit rating of player
                                    // Returns the critical chance for the caster weapon
        };

        static Func<float, float, float, int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(WeaponCriticalChance));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool PreGetWeaponCriticalChance(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref WorldObject __instance, ref float __result)
        {
            var critRate = (float)(weapon?.CriticalFrequency ?? .1f);

            //Use imbue bonus if larger
            float criticalStrikeBonus = 0;
            if (weapon != null && weapon.HasImbuedEffect(ImbuedEffectType.CriticalStrike))
            {
                criticalStrikeBonus = WorldObject.GetCriticalStrikeMod(skill);
            }
            var max = Math.Max(critRate, criticalStrikeBonus);

            var rating = wielder is null ? 1 : wielder.GetCritRating();

            __result = func(max, critRate, criticalStrikeBonus, rating);

            return false;
        }
        #endregion
    }
}