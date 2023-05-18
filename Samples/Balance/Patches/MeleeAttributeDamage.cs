namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(MeleeAttributeDamage))]
    public class MeleeAttributeDamage : AngouriMathPatch
    {
        #region Fields / Props   
        //Named property used to indicate patch and enable in settings
        public override bool Enabled { get; set; } = true;

        //Formula and the variables used
        //x = skill, n = number of connections  -- Originally Math.Max(1.0f + (currentSkill - 55) * .011, 1.0f)
        [JsonPropertyName($"Formula")]
        public override string Formula { get; set; } = "P(1 if x < 55, 1+.011(x-55))";
        [JsonInclude]
        public override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,
            ["n"] = MType.Int,
        };

        static Func<int, int, float> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction<int, int, float>(out func, Variables.TypesAndNames()))
                Mod.Harmony.PatchCategory(nameof(MeleeAttributeDamage));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }

        public override void Shutdown()
        {
            func = null;
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Creature), nameof(Creature.GetAttributeMod), new Type[] { typeof(WorldObject) })]
        public static bool PreGetAttributeMod(WorldObject weapon, ref Creature __instance, ref float __result)
        {
            if (__instance is Player player)
            {
                //Only melee
                var isBow = weapon != null && weapon.IsBow;                
                if (isBow) return true;

                var attribute = isBow || weapon?.WeaponSkill == Skill.FinesseWeapons ? __instance.Coordination : __instance.Strength;

                __result = func((int)attribute.Current, player.ActiveConnections());
                //SkillFormula.GetAttributeMod((int)attribute.Current, isBow);

                return false;
            }

            return true;
        }
        #endregion
    }
}