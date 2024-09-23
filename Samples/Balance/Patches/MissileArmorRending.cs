using static ACE.Server.WorldObjects.WorldObject;

namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(MissileArmorRending))]
    public class MissileArmorRending : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "P(.6 if (x-160)/400 > .6, (x-160)/400)";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,  // base skill
                                // % of armor ignored, min 0%, max 60%
        };

        static Func<int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(MissileArmorRending));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(GetArmorRendingMod), new Type[] { typeof(CreatureSkill) })]
        public static bool PreGetArmorRendingMod(CreatureSkill skill, ref WorldObject __instance, ref float __result)
        {
            if (GetImbuedSkillType(skill) != ImbuedSkillType.Melee)
                return true;

            var baseSkill = GetBaseSkillImbued(skill);
            __result = func(baseSkill);

            return false;
        }
        #endregion
    }
}