namespace ACE.Shared.Helpers;

public static class BiotaExtensions
{
    public static bool IsNpc(this ACE.Entity.Models.Biota biota)
    {
        //Assume npc
        if (biota is null || biota.PropertiesBool is null || biota.PropertiesInt is null) return true;

        //IsNPC => !(this is Player) && !Attackable && TargetingTactic == TargetingTactic.None;
        //Attackable = GetProperty(PropertyBool.Attackable) ?? true;
        if (!biota.PropertiesBool.TryGetValue(PropertyBool.Attackable, out var value) || value != false)
            return false;

        //TargetingTactic = (TargetingTactic)(GetProperty(PropertyInt.TargetingTactic) ?? 0);
        if (!biota.PropertiesInt.TryGetValue(PropertyInt.TargetingTactic, out var target) || target != (int)ACE.Entity.Enum.TargetingTactic.None)
            return false;

        return true;
    }
}
