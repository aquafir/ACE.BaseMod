using ACE.Server.Entity;
using ACE.Server.Entity.Actions;

namespace Bank;

[HarmonyPatch]
internal class CoinDrop
{
    //Create a fake property bool to track
    static PropertyBool SonicProp = (PropertyBool)39998;

    //Create a command to toggle the variable
    [CommandHandler("gottagofast", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT2(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;

        //Get the current property
        var sonicMode = player.GetProperty(SonicProp) ?? false;

        //Toggle the prop
        sonicMode = !sonicMode;

        //Set it to the opposite and inform the player
        player.SetProperty(SonicProp, sonicMode);
        player.SendMessage($"Corpses {(sonicMode ? "explode" : "don't explode")}.");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetNumCoinsDropped))]
    public static void PostGetNumCoinsDropped(ref Player __instance, ref int __result)
    {
        //Cap coins dropped
        if (PatchClass.Settings.MaxCoinsDropped < 0)
            return;

        __result = Math.Min(__result, PatchClass.Settings.MaxCoinsDropped);
        if (PatchClass.Settings.CoinExplosion && __instance.GetProperty(SonicProp) != false)
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


        var actionChain = new ActionChain();

        actionChain.EnqueueChain();
        int pilesMade = 0;
        while (coins > 0 && pilesMade++ < PatchClass.Settings.MaxPiles)
        {
            coins -= PatchClass.Settings.PileValue;
            var (x, y) = GetSpiralCoordinate(pilesMade, velocity);
            Vector3 explosion = new((float)x, (float)y, (float)ThreadSafeRandom.Next(0, velocity));

            actionChain.AddDelaySeconds(.1);
            actionChain.AddAction(player, () =>
            {
                var cash = WorldObjectFactory.CreateNewWorldObject(PatchClass.Settings.PileWcid);

                //Get stack size.  Higher denominations..?
                //var amt = Math.Min(coins, PatchClass.Settings.PileValue);
                //coins -= amt;

                //Set stack size only of pyreals?
                //if(cash.MaxStackSize is not null)
                if (PatchClass.Settings.PileWcid == Player.coinStackWcid)
                    cash.StackSize = cash.MaxStackSize;

                //Center on player
#if REALM
                cash.Location = player.PhysicsObj.Position.ACEPosition(player.Location.Instance);
#else
                cash.Location = player.PhysicsObj.Position.ACEPosition();
#endif
                cash.InitPhysicsObj();

                //Explode from player
                //Vector3 explosion = new((float)ThreadSafeRandom.Next(-velocity, velocity), (float)ThreadSafeRandom.Next(-velocity, velocity), (float)ThreadSafeRandom.Next(-velocity, velocity));
                cash.PhysicsObj.Velocity = player.PhysicsObj.Velocity + explosion;

                cash.EnterWorld();
            });

        }
        actionChain.EnqueueChain();
    }

    const double ANGLE_INCREMENT = Math.PI / 6; // 30 degrees in radians
    public static (double x, double y) GetSpiralCoordinate(int i, float scale = 5)
    {
        //Get rotation and distance from origin relative to i
        double radius = Math.Sqrt(i) * scale;
        double angle = i * ANGLE_INCREMENT;

        //Calculate x/y components
        double x = radius * Math.Cos(angle);
        double y = radius * Math.Sin(angle);

        return (x, y);
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
