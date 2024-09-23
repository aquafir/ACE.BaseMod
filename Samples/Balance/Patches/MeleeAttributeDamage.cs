namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(MeleeAttributeDamage))]
public class MeleeAttributeDamage : AngouriMathPatch
{
    #region Fields / Props   
    //Originally Math.Max(1.0f + (currentSkill - 55) * .011, 1.0f)
    public override string Formula { get; set; } = "P(1 if x < 55, 1+.011(x-55))";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Int,  // skill
        ["n"] = MType.Int,  // # connections
    };

    static Func<int, int, float> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
            Mod.Instance.Harmony.PatchCategory(nameof(MeleeAttributeDamage));
        else
            throw new Exception($"Failure parsing formula: {Formula}");
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GetAttributeMod), new Type[] { typeof(WorldObject) })]
    public static bool PreGetAttributeMod(WorldObject weapon, ref Creature __instance, ref float __result)
    {
        //In the process of changing settings func will be null
        //if (func is null) return true;

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