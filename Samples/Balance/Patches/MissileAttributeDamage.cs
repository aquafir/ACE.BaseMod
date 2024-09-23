namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(MissileAttributeDamage))]
public class MissileAttributeDamage : AngouriMathPatch
{
    #region Fields / Props   
    //Originally Math.Max(1.0f + (currentSkill - 55) * .008, 1.0f)
    public override string Formula { get; set; } = "P(1 if x < 55, 1+.008(x-55))";
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
            Mod.Instance.Harmony.PatchCategory(nameof(MissileAttributeDamage));
        else
            throw new Exception($"Failure parsing formula: {Formula}");

    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GetAttributeMod), new Type[] { typeof(WorldObject) })]
    public static bool PreGetAttributeMod(WorldObject weapon, ref Creature __instance, ref float __result)
    {
        if (__instance is Player player)
        {
            //Only bow
            var isBow = weapon != null && weapon.IsBow;
            if (!isBow) return true;

            var attribute = isBow || weapon?.WeaponSkill == Skill.FinesseWeapons ? __instance.Coordination : __instance.Strength;

            __result = func((int)attribute.Current, player.ActiveConnections());
            //SkillFormula.GetAttributeMod((int)attribute.Current, isBow);

            return false;
        }

        return true;
    }
    #endregion
}