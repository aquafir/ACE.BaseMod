namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(MagicWeaponCriticalChance))]
    public class MagicWeaponCriticalChance : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "x + .01r";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Float,    // Larger of the two bonuses, before rating is added
            ["f"] = MType.Float,    // Weapon frequency or .1 if null
            ["c"] = MType.Float,    // Imbue modifier or 0 if not imbued
            ["r"] = MType.Int,      // Crit rating of player
                                    // Critical chance for the attack weapon
        };

        static Func<float, float, float, int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(MagicWeaponCriticalChance));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponMagicCritFrequency), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool PreGetWeaponMagicCritFrequency(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref WorldObject __instance, ref float __result)
        {
            //Skip without weapon?
            if (weapon == null) return true;

            var critRate = (float)(weapon.GetProperty(PropertyFloat.CriticalFrequency) ?? .05);

            var criticalStrikeMod = 0f;
            if (weapon.HasImbuedEffect(ImbuedEffectType.CriticalStrike))
            {
                var isPvP = wielder is Player && target is Player;

                criticalStrikeMod = WorldObject.GetCriticalStrikeMod(skill, isPvP);
            }
            var max = Math.Max(critRate, criticalStrikeMod);

            var rating = wielder.GetCritRating();

            __result = func(max, critRate, criticalStrikeMod, rating);

            return false;
        }
        #endregion
    }
}