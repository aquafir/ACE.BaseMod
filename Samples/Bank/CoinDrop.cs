using ACE.Common;
using ACE.Server.Factories;
using ACE.Server.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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
        if (PatchClass.Settings.CoinExplosion)
        {
            //Explode coins
            CoinExplosion(__instance, __result, PatchClass.Settings.ExplosionVelocity);

            //Remove coins immediately and zero the dropped amount
            __instance.SpendCurrency(Player.coinStackWcid, (uint)__result);
            __result = 0;
        }
    }


    private static void CoinExplosion(Player player, int coins, float velocity = 5f)
    {
        if (player == null) return;

        int pilesMade = 0;
        while (coins > 0 && pilesMade++ < PatchClass.Settings.MaxPiles)
        {
            var cash = WorldObjectFactory.CreateNewWorldObject(PatchClass.Settings.PileWcid);

            //Get stack size.  Higher denominations..?
            //var amt = Math.Min(coins, PatchClass.Settings.PileValue);
            //coins -= amt;
            coins -= PatchClass.Settings.PileValue;

            //Set stack size only of pyreals?
            //if(cash.MaxStackSize is not null)
            if(PatchClass.Settings.PileWcid == Player.coinStackWcid)
                cash.StackSize = cash.MaxStackSize;

            //Center on player
            cash.Location = player.PhysicsObj.Position.ACEPosition();
            cash.InitPhysicsObj();

            //Explode from player
            Vector3 explosion = new((float)ThreadSafeRandom.Next(-velocity, velocity), (float)ThreadSafeRandom.Next(-velocity, velocity), (float)ThreadSafeRandom.Next(-velocity, velocity));
            cash.PhysicsObj.Velocity = player.PhysicsObj.Velocity + explosion;

            cash.EnterWorld();
        }
    }

    [CommandHandler("coinsplode", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT1(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (parameters.Length < 2)
        {
            CoinExplosion(player, 500000, PatchClass.Settings.ExplosionVelocity);
            return;
        }
        if (!int.TryParse(parameters[0], out var amt) || !float.TryParse(parameters[1], out var velocity)) 
            return;

        CoinExplosion(player, amt, velocity);
    }
}
