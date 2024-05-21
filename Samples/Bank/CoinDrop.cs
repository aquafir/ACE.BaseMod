using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank;

[HarmonyPatch]
internal class CoinDrop
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetNumCoinsDropped))]
    public static void PostGetNumCoinsDropped(ref Player __instance, ref int __result)
    {
        //Cap coins dropped
        if (PatchClass.Settings.MaxCoinsDropped < 0)
            return;

        __result = Math.Min(__result, PatchClass.Settings.MaxCoinsDropped);
    }
}
