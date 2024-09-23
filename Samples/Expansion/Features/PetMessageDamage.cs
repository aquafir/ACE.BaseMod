namespace Expansion.Features;

[CommandCategory(nameof(Feature.PetMessageDamage))]
[HarmonyPatchCategory(nameof(Feature.PetMessageDamage))]
internal class PetMessageDamage
{
    //Creature.MeleeAttack calls on dealing/receiving combat pet damage
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(bool) })]
    public static void PostTakeDamage(WorldObject source, DamageType damageType, float amount, bool crit, ref Creature __instance, ref uint __result)
    {
        if (source is Pet target)
        {
            //var hp = __instance.Health.Current;
            //var alive = __instance.IsAlive;
            if (!__instance.IsAlive)
                target.P_PetOwner.SendMessage($"Your {source.Name} has slain {__instance.Name}.");
            else
                target.P_PetOwner.SendMessage($"Your {source.Name} has {(crit ? "critically " : "")}hit {__instance.Name} for {(int)amount} {damageType} damage.", ChatMessageType.CombatSelf);
            //target.P_PetOwner.SendMessage($"Your pet has {(crit ? "critically " : "")}hit {target.Name} for {amount}.", ChatMessageType.Combat);
        }
        if (__instance is Pet pet)
        {
            //if (pet.Health.Current > amount)
            pet.P_PetOwner.SendMessage($"{pet.Name} has been {(crit ? "critically " : "")}hit for {(int)amount} by {source.Name} {damageType} damage.", ChatMessageType.CombatEnemy);
            //else
            //    pet?.P_PetOwner?.SendMessage($"Your {pet.Name} has been killed by {source.Name}.");
        }
    }
}
