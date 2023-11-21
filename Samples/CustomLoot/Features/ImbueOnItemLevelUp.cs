using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Features;
internal class ImbueOnItemLevelUp
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnItemLevelUp), new Type[] { typeof(WorldObject), typeof(int) })]
    public static bool PreOnItemLevelUp(WorldObject item, int prevItemLevel, ref Player __instance)
    {
        //Return false to override
        //return false;

        //Return true to execute original
        return true;
    }
}
