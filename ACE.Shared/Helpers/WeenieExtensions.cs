using ACE.Entity.Models;

namespace ACE.Shared.Helpers;

public static class WeenieExtensions
{
    public static bool IsNpc(this Weenie weenie)
    {
        //Assume npc
        if (weenie is null) return true;

        //Check NPC as no target, unattackable
        var target = weenie.GetProperty(PropertyInt.TargetingTactic);
        if (target is null || target != (int)ACE.Entity.Enum.TargetingTactic.None)
            return false;

        if (weenie.GetProperty(PropertyBool.Attackable) ?? true)
            return false;

        return true;
    }
}
