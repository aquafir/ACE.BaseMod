namespace Expansion.Features;

[CommandCategory(nameof(Feature.CreatureEx))]
[HarmonyPatchCategory(nameof(Feature.CreatureEx))]
public class CreatureEx
{
    //Replace Factory creation of creatures with a random chance of a CreatureEx
    [HarmonyPrefix]
#if REALM
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid), typeof(AppliedRuleset) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset, ref WorldObject __result)
#else
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, ref WorldObject __result)
#endif
    {
        if (weenie.WeenieType != WeenieType.Creature || weenie == null) return true;

        //Check for a specified CreatureEx type
        if (weenie.PropertiesInt.TryGetValue((PropertyInt)FakeInt.CreatureExType, out var creatureType))
        {
            var type = (CreatureExType)creatureType;
            if (!Enum.IsDefined(type) || type == CreatureExType.Unknown)
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

        //Fix NPCs being created as CreatureEx in post!
        if (__result is Creature creature && creature.IsNPC)
#if REALM
            __result = new Creature(weenie, guid, ruleset);
#else
            __result = new Creature(weenie, guid);
#endif
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Biota) })]
    public static bool PreCreateWorldObject(Biota biota, ref WorldObject __result)
    {
        if (biota.WeenieType != WeenieType.Creature) return true;

        //Check for a specified CreatureEx type
        if (biota.PropertiesInt.TryGetValue((PropertyInt)FakeInt.CreatureExType, out var creatureType))
        {
            var type = (CreatureExType)creatureType;
            if (!Enum.IsDefined(type) || type == CreatureExType.Unknown)
            {
                ModManager.Log($"Invalid CreatureEx type found: {type}");
                return false;
            }

            __result = type.Create(biota);
        }
        //Roll for a random type
        else if (ThreadSafeRandom.Next(0, 1.0f) <= PatchClass.Settings.CreatureChance)
        {
            __result = RollCreature(biota);
        }

        //Fix NPCs being created as CreatureEx in post!
        if (__result is Creature creature && creature.IsNPC)
            __result = new Creature(biota);

        return false;
    }


    //Todo: decide on getting random from the enum values vs. an expected offset
    protected readonly static int _possibleCreatureTypes = Enum.GetValues<CreatureExType>().Length;
    protected static CreatureExType RandomCreatureType() => (CreatureExType)ThreadSafeRandom.Next(0, _possibleCreatureTypes);

    public static Creatures.CreatureEx RollCreature(Weenie weenie, ObjectGuid guid) => RandomCreatureType().Create(weenie, guid);
    public static Creatures.CreatureEx RollCreature(Biota biota) => RandomCreatureType().Create(biota);
}