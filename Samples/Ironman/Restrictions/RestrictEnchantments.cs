namespace Ironman.Restrictions;

[HarmonyPatchCategory(nameof(RestrictEnchantments))]
public static class RestrictEnchantments
{
    //Check enchantments
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CreateEnchantment), new Type[] { typeof(WorldObject), typeof(WorldObject), typeof(WorldObject), typeof(Spell), typeof(bool), typeof(bool), typeof(bool) })]
    public static bool PreCreateEnchantment(WorldObject target, WorldObject caster, WorldObject weapon, Spell spell, bool equip, bool fromProc, bool isWeaponSpell, ref WorldObject __instance)
    {
        if (target is Player player && player.GetProperty(FakeBool.Ironman) == true && caster.GetProperty(FakeBool.Ironman) != true)
        {
            player.SendMessage($"{caster.Name} failed to cast {spell.Name ?? ""} on you.  Blame Ironmode");

            if (caster is Player p)
                p.SendMessage($"Failed to cast {spell.Name ?? ""} on {player.Name}. Blame Ironmode");

            return false;
        }

        return true;
    }

}
//[HarmonyPatchCategory(nameof(Restrict))]
//public static class Restrict
//{
//}
