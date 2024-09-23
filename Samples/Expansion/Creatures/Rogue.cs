namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Rogue))]
public class Rogue : CreatureEx
{
    public Rogue(Biota biota) : base(biota) { }
#if REALM
    public Rogue(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Rogue(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Rakish " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DamageEvent), "DoCalculateDamage", new Type[] { typeof(Creature), typeof(Creature), typeof(WorldObject) })]
    public static void PostDoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is not Rogue c || defender is not Player p) return;
        if (!__instance.HasDamage) return;

        var angle = p.GetAngle(c);
        var behind = Math.Abs(angle) > 90.0f;
        if (!behind)
            return;

        //Start with equipped
        var item = p.EquippedObjects.Values.FirstOrDefault();

        //Then try inventory
        if (item is null)
            item = p.Inventory.Values.FirstOrDefault();

        //Cancel if there's no items
        if (item is null) return;

        p.SendMessage($"Try to fumble {item.Name}");
        if (p.TryDropItem(item))
        {
            p.SendMessage($"{c.Name} has disarmed your {item.Name}!");
        }
    }
}