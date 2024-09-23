namespace Expansion.Features;

[CommandCategory(nameof(Feature.PetExShareDamage))]
[HarmonyPatchCategory(nameof(Feature.PetExShareDamage))]
public static class PetExShareDamage
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CombatPetEx), nameof(CombatPetEx.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(bool) })]
    public static void PreTakeDamage(WorldObject source, DamageType damageType, ref float amount, bool crit, ref CombatPetEx __instance, ref uint __result)
    {
        if (__instance?.P_PetOwner is not Player player)
            return;

        var ownerAmt = amount * .1f;
        amount *= .9f;
        player.TakeDamage(source, damageType, ownerAmt, crit);
        player.SendMessage($"You've been {(crit ? "critically " : "")} hit for {(int)ownerAmt} {damageType} damage by {source.Name} through your link with {__instance.Name}.", ChatMessageType.CombatEnemy);
    }
}
