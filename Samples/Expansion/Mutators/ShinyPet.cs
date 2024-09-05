using ACE.Database;

namespace Expansion.Mutators;

public class ShinyPet : Mutator
{

    public override bool CheckMutatesLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item = null)
    {
        return roll.ItemType == TreasureItemType_Orig.PetDevice;
    }
    public override bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item)
    {
        //Doesn't mutate Slayers
        if (item is not PetDevice petDevice)
            return false;

        petDevice.Name = "Shiny " + petDevice.Name;
        petDevice.MaxStructure = 250;

        //petDevice.GearDamage;
        //petDevice.GearDamageResist;
        //petDevice.GearCritDamage;
        //petDevice.GearCritDamageResist;
        petDevice.GearCrit = 100;
        //petDevice.GearCritResist;

        //Lazy way of doing stuff after it spawns in its container
        var actionChain = new ActionChain();
        actionChain.AddDelaySeconds(3.0f);  // wait for container placement
        actionChain.AddAction(item, () =>
        {
            if (item.Container is not Corpse corpse)
                return;

            if (PlayerManager.FindByGuid(corpse.KillerId ?? 0) is not Player player)
                return;

            if (item is not PetDevice petDevice)
                return;

            var livingType = corpse.GetProperty(FakeInt.CorpseLivingWCID);
            if (livingType is null)
                return;

            var weenie = DatabaseManager.World.GetCachedWeenie((uint)livingType);
            if (weenie is null)
                return;

            //            if (corpse.CreatureType is null)
            //return;

            player.DoWorldBroadcast($"A shiny {weenie.GetName()} has dropped from a {corpse.Name} killed by {player.Name}!!", ChatMessageType.WorldBroadcast);
            petDevice.PetClass = livingType;
            petDevice.Structure = 250;
            petDevice.CooldownDuration = 5;
            petDevice.IconId = weenie.GetProperty(PropertyDataId.Icon) ?? petDevice.IconId;
            petDevice.RemoveProperty(PropertyInt.SummoningMastery);

            petDevice.UseRequiresLevel = null;
            petDevice.UseRequiresSkillLevel = null;
            petDevice.UseRequiresSkill = null;

            petDevice.Name = $"Shiny {corpse.Name}";
            //var parent = item.Container;

        });
        actionChain.EnqueueChain();

        return true;
    }

    /// <summary>
    /// Example of doing some setup when the Mutator is created
    /// </summary>
    public override void Start()
    {

    }
}