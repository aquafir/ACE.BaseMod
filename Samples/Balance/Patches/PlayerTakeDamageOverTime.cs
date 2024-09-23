namespace Balance.Patches
{
    [HarmonyPatch]
    [HarmonyPatchCategory(nameof(PlayerTakeDamageOverTime))]
    public class PlayerTakeDamageOverTime : AngouriMathPatch
    {
        #region Fields / Props   
        public override string Formula { get; set; } = "x/3n";
        protected override Dictionary<string, MType> Variables { get; } = new()
        {
            ["x"] = MType.Float,    // Incoming DoT damage
            ["n"] = MType.Int,      // # connections
                                    // Modified damage taken by player
        };

        static Func<float, int, int> func;
        #endregion

        #region Start / Stop
        public override void Start()
        {
            //If you can parse the formulas patch the corresponding category
            if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
                Mod.Instance.Harmony.PatchCategory(nameof(PlayerTakeDamageOverTime));
            else
                throw new Exception($"Failure parsing formula: {Formula}");
        }
        #endregion

        #region Patches
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.TakeDamageOverTime), new Type[] { typeof(float), typeof(DamageType) })]
        public static bool PreTakeDamageOverTime(ref float _amount, DamageType damageType, ref Player __instance)
        {
            _amount = func(_amount, __instance.ActiveConnections());

            //Return true to execute original
            return true;
        }

        #endregion
    }
}