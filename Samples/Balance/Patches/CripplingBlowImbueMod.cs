using static ACE.Server.WorldObjects.WorldObject;

namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(CripplingBlowImbueMod))]
    public class CripplingBlowImbueMod : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "P((x-40)/60 if t=1, x/60)";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,  // Skill
            ["t"] = MType.Int,  // ImbuedSkillType (Melee = 1, Missile = 2, Magic = 3) 
                                // increases the critical damage multiplier, additive
        };

        static Func<int, int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(CripplingBlowImbueMod));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(GetCripplingBlowMod), new Type[] { typeof(CreatureSkill) })]
        public static bool PreGetCripplingBlowMod(CreatureSkill skill, ref WorldObject __instance, ref float __result)
        {
            var baseSkill = GetBaseSkillImbued(skill);
            __result = func(baseSkill, (int)GetImbuedSkillType(skill));

            //Check for a min?
            __result = Math.Max(1.0f, __result);

            return false;
        }

        #endregion
    }
}