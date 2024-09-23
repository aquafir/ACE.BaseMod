using static ACE.Server.WorldObjects.WorldObject;

namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(CriticalStrikeImbueMod))]
    public class CriticalStrikeImbueMod : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "P((x-100)/600 if t=1, (x-60)/600 if t>1, 0)";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,  // Skill
            ["t"] = MType.Int,  // ImbuedSkillType (Melee = 1, Missile = 2, Magic = 3) 
                                // Crit chance, floor of .05/.1 for ranged/melee
        };

        static Func<int, int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(CriticalStrikeImbueMod));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(GetCriticalStrikeMod), new Type[] { typeof(CreatureSkill), typeof(bool) })]
        public static bool PreGetCriticalStrikeMod(CreatureSkill skill, bool isPvP, ref WorldObject __instance, ref float __result)
        {
            //Don't apply to pvp
            if (isPvP) return true;

            //Mod=.5
            var skillType = GetImbuedSkillType(skill);
            var baseSkill = GetBaseSkillImbued(skill);

            __result = func(baseSkill, (int)skillType);

            //Use mins as floor?
            var defaultCritFrequency = skillType == ImbuedSkillType.Magic ? .05f : .1f;
            __result = Math.Max(defaultCritFrequency, __result);

            //Return false to override
            return false;
        }

        #endregion
    }
}