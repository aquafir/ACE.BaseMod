﻿namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureEx))]
public class Tank : CreatureEx
{
    public Tank(Biota biota) : base(biota) { }
    public Tank(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Guardian " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    const float range = 10f;
    const int candidates = 4;
    const float tankMultiplier = .2f;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(bool) })]
    public static void PreTakeDamage(WorldObject source, DamageType damageType, ref float amount, bool crit, ref Creature __instance, ref uint __result)
    {
        if(source is not Player p || __instance is Tank)
            return;

        //If there is a Tank nearby swap that for the target dealt damage and mitigate
        var nearby = p.GetNearbyCreatures(__instance, candidates, range);
        foreach (var creature in nearby)
        {
            if(creature is Tank)
            {
                p.SendMessage($"{creature.Name} tanked the {amount} damage.");
                amount *= tankMultiplier;
                __instance = creature;
                return;
            }
        }
    }
}