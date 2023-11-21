namespace CustomLoot.Mutators;

public class Slayer : Mutator
{

    public override void Start()
    {
        //base.Start();
    }
    public static void HandleSlayerMutation(TreasureDeath treasureDeath, WorldObject __result)
    {
        //if (ThreadSafeRandom.Next(0.0f, 1.0f) < PatchClass.Settings.SlayerChance)
        //{
        //    //Check already slayer            
        //    if (__result.GetProperty(PropertyInt.SlayerCreatureType) is not null)
        //        return;

        //    //Use all creatures or just a subset
        //    var cTypes = PatchClass.Settings.UseCustomSlayers ? PatchClass.Settings.SlayerSpecies : Enum.GetValues<CreatureType>();

        //    if (cTypes.Length < 1)
        //    {
        //        ModManager.Log("No available species to add Slayer for.");
        //        return;
        //    }

        //    //Get a random type
        //    var type = cTypes[ThreadSafeRandom.Next(0, cTypes.Length - 1)];
        //    var power = PatchClass.Settings.SlayerPower[treasureDeath.Tier];

        //    __result.SetProperty(PropertyInt.SlayerCreatureType, (int)type);
        //    __result.SetProperty(PropertyFloat.SlayerDamageBonus, power);
        //}
    }
}