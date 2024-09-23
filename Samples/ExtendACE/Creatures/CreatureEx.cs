using static ACE.Server.Factories.PlayerFactory;
using Weenie = ACE.Entity.Models.Weenie;

namespace Tinkering.Creatures;

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

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    //public static void PostCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    public static bool PreCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
    {
        //Return false to override
        //return false;

        //Return true to execute original
        return true;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    public static void PostCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
    {
        //Your code here
    }



    //Replace Factory creation of creatures
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, ref WorldObject __result)
    {
        //if (weenie.IsNpc()) return true;
        if (weenie.WeenieType != WeenieType.Creature || weenie == null) return true;
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

        //Fix it in post!
        var timer = Stopwatch.StartNew();
        if (__result is Creature creature && creature.IsNPC)
        {
            __result = new Creature(biota);
            timer.Stop();
            ModManager.Log($"{creature.Name} was an NPC remade as a vanilla Creature in {timer.ElapsedMilliseconds} ms");
        }

        return false;
    }

    protected static int possibleCreatureTypes = Enum.GetValues<CreatureType>().Length;
    protected static CreatureType RandomCreatureType() => (CreatureType)ThreadSafeRandom.Next(0, possibleCreatureTypes);
    //Creatures.CreatureType.Boss; 

    public static CreatureEx RollCreature(Weenie weenie, ObjectGuid guid) => RandomCreatureType().Create(weenie, guid);
    public static CreatureEx RollCreature(Biota biota) => RandomCreatureType().Create(biota);
}
