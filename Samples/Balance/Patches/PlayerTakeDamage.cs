namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(PlayerTakeDamage))]
    public class PlayerTakeDamage : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "n*x/3";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Int,  // Damage taken
            ["n"] = MType.Int,  // # connections
                                // Modified damage taken by player
        };

        static Func<int, int, int> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(PlayerTakeDamage));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), nameof(Player.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(BodyPart), typeof(bool), typeof(AttackConditions) })]
        public static void PostTakeDamage(WorldObject source, DamageType damageType, float _amount, BodyPart bodyPart, bool crit, AttackConditions attackConditions, ref Player __instance, ref int __result)
        {
            __result = func(__result, __instance.ActiveConnections());
        }
        #endregion
    }
}
