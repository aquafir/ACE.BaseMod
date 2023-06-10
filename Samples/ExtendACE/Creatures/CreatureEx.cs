using System;
using Weenie = ACE.Entity.Models.Weenie;

namespace ExtendACE.Creatures;

[HarmonyPatch]
public class CreatureEx : Creature
{
    public CreatureEx(Biota biota) : base(biota)
    {
        Initialize();
    }
    public CreatureEx(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
    {
        Initialize();
    }
    /// <summary>
    /// Called upon construction to set up creature
    /// </summary>
    protected virtual void Initialize() { }

    //Replace Factory creation of creatures
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, ref WorldObject __result)
    {
        if (weenie.WeenieType != WeenieType.Creature || weenie == null) return true;
        if (ThreadSafeRandom.Next(0, 1.0f) > PatchClass.Settings.CreatureChance) return true;

        __result = RollCreature(weenie, guid);

        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Biota) })]
    public static bool PreCreateWorldObject(Biota biota, ref WorldObject __result)
    {
        if (biota.WeenieType != WeenieType.Creature) return true;
        if (ThreadSafeRandom.Next(0, 1.0f) > PatchClass.Settings.CreatureChance) return true;

        __result = RollCreature(biota);

        return false;
    }

    protected static int possibleCreatureTypes = Enum.GetValues<CreatureType>().Length;
    protected static CreatureType RandomCreatureType() => (CreatureType) ThreadSafeRandom.Next(0, possibleCreatureTypes);

    public static CreatureEx RollCreature(Weenie weenie, ObjectGuid guid) => new CreatureEx(weenie, guid);
        //RandomCreatureType().Create(weenie, guid);
    public static CreatureEx RollCreature(Biota biota) => RandomCreatureType().Create(biota);
}
