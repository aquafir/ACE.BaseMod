using static ACE.Server.WorldObjects.WorldObject;

namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(ElementalRendingImbue))]
    public class ElementalRendingImbue : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "P(x/160 if t=1, x/144)";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,  // Skill
            ["t"] = MType.Int,  // ImbuedSkillType (Melee = 1, Missile = 2, Magic = 3) 
                                // Elemental rending damage multiplier
        };

        static Func<int, int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(ElementalRendingImbue));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(GetRendingMod), new Type[] { typeof(CreatureSkill) })]
        public static bool PreGetRendingMod(CreatureSkill skill, ref WorldObject __instance, ref float __result)
        {
            // elemental rending cap, equivalent to level 6 vuln
            //        public static float MaxRendingMod = 2.5f;
            var baseSkill = GetBaseSkillImbued(skill);
            __result = func(baseSkill, (int)GetImbuedSkillType(skill));

            //Don't clamp but set a floor
            __result = Math.Max(__result, 1.0f);

            return false;
        }

        #endregion
    }
}