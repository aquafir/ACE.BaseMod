using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expansion.Features;

[HarmonyPatchCategory(nameof(Feature.CreatureEx))]
public class CreatureEx
{
    //Replace Factory creation of creatures with a random chance of a CreatureEx
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, ref WorldObject __result)
    {
        if (weenie.WeenieType != WeenieType.Creature || weenie == null) return true;

        //Check for a specified CreatureEx type
        if(weenie.PropertiesInt.TryGetValue((PropertyInt)FakeInt.CreatureExType, out var creatureType))
        {
            var type = (CreatureExType)creatureType;
            if(!Enum.IsDefined(type))
            {
                ModManager.Log($"Invalid CreatureEx type found: {type}");
                return true;
            }

            __result = type.Create(weenie, guid);
            return false;
        }

        //Roll for a random type
        if (ThreadSafeRandom.Next(0, 1.0f) > PatchClass.Settings.CreatureChance) return true;

        __result = RollCreature(weenie, guid);

        //Fix it in post!
        var timer = Stopwatch.StartNew();
        if (__result is Creature creature && creature.IsNPC)
        {
            __result = new Creature(weenie, guid);
            timer.Stop();
            ModManager.Log($"{creature.Name} was an NPC remade as a vanilla Creature in {timer.ElapsedMilliseconds} ms");
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Biota) })]
    public static bool PreCreateWorldObject(Biota biota, ref WorldObject __result)
    {
        //if (biota.IsNpc()) return true;
        if (biota.WeenieType != WeenieType.Creature) return true;
        if (ThreadSafeRandom.Next(0, 1.0f) > PatchClass.Settings.CreatureChance) return true;
        __result = RollCreature(biota);

        //Fix NPCs being created as CreatureEx in post!
        var timer = Stopwatch.StartNew();
        if (__result is Creature creature && creature.IsNPC)
        {
            __result = new Creature(biota);
            timer.Stop();
            ModManager.Log($"{creature.Name} was an NPC remade as a vanilla Creature in {timer.ElapsedMilliseconds} ms");
        }

        return false;
    }


    //Todo: decide on getting random from the enum values vs. an expected offset
    protected readonly static int _possibleCreatureTypes = Enum.GetValues<CreatureExType>().Length;
    protected static CreatureExType RandomCreatureType() => (CreatureExType)ThreadSafeRandom.Next(0, _possibleCreatureTypes);

    public static Creatures.CreatureEx RollCreature(Weenie weenie, ObjectGuid guid) => RandomCreatureType().Create(weenie, guid);
    public static Creatures.CreatureEx RollCreature(Biota biota) => RandomCreatureType().Create(biota);
}