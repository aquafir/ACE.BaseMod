namespace Expansion.Features;

[CommandCategory(nameof(Feature.PetEx))]
[HarmonyPatchCategory(nameof(Feature.PetEx))]
public static class PetEx
{
    //Replace Factory creation of creatures with a random chance of a CreatureEx
    #region Construction
    [HarmonyPrefix]
#if REALM
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid), typeof(AppliedRuleset) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset, ref WorldObject __result)
#else
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid) })]
    public static bool PreCreateWorldObject(Weenie weenie, ObjectGuid guid, ref WorldObject __result)
#endif
    {
        if (weenie.WeenieType != WeenieType.CombatPet || weenie is null)
            return true;

#if REALM
        __result = new CombatPetEx(weenie, guid, ruleset);
#else
        __result = new CombatPetEx(weenie, guid);
#endif

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Biota) })]
    public static bool PreCreateWorldObject(Biota biota, ref WorldObject __result)
    {
        if (biota.WeenieType != WeenieType.CombatPet || biota is null)
            return true;

        __result = new CombatPetEx(biota);

        return false;
    }
    #endregion

    //Tell owner about damage pet deals
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(bool) })]
    public static void PostTakeDamage(WorldObject source, DamageType damageType, float amount, bool crit, ref Creature __instance, ref uint __result)
    {
        if (source is Pet target)
        {
            if (!__instance.IsAlive)
                target.P_PetOwner.SendMessage($"Your {source.Name} has slain {__instance.Name}.");
            else
                target.P_PetOwner.SendMessage($"Your {source.Name} has {(crit ? "critically " : "")}hit {__instance.Name} for {(int)amount} {damageType} damage.", ChatMessageType.CombatSelf);
        }
    }
}

public class CombatPetEx : CombatPet
{
    public CombatPetEx(Biota biota) : base(biota)
    {
        Setup();
    }

#if REALM
    public CombatPetEx(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public CombatPetEx(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    {
        Setup();
    }

    private void Setup()
    {
        //Set visibility?
        RadarBehavior = ACE.Entity.Enum.RadarBehavior.ShowAlways;
        RadarColor = ACE.Entity.Enum.RadarColor.Green;
        Name += " (Ex)";
    }

    public override uint TakeDamage(WorldObject source, DamageType damageType, float amount, bool crit = false)
    {
        if (source is not null)
            P_PetOwner.SendMessage($"{Name} has been {(crit ? "critically " : "")}hit for {(int)amount} by {source.Name} {damageType} damage.", ChatMessageType.CombatEnemy);

        return base.TakeDamage(source, damageType, amount, crit);
    }

    public override void Die(DamageHistoryInfo lastDamager, DamageHistoryInfo topDamager)
    {
        P_PetOwner.SendMessage($"{Name} has been been killed by {topDamager.Name ?? "Unknown"}.", ChatMessageType.System);

        base.Die(lastDamager, topDamager);
    }


    const int ORPHAN_HEARTBEATS = 5;
    int idleCount = 0;
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (P_PetOwner is not null && (AttackTarget is null || AttackTarget.IsDestroyed))
        {
            //Teleport if too far?
            var dist = P_PetOwner.GetCylinderDistance(this);
            if (dist > 15)
            {
                //TODO: Figure out how to handle teleport
#if REALM
                if (P_PetOwner.Location.InstancedLandblock == Location.InstancedLandblock)
#else
                if(true)
#endif
                {
                    FakeTeleport(P_PetOwner.Location.InFrontOf(.1));
                    //P_PetOwner.SendMessage($"{Name} has caught up to you.");
                }
                else if (!IsMoving)
                    MoveTo(P_PetOwner);
            }
            else if (dist > 250)
                Destroy();
        }
        //Add check to destroy orphaned pets?
        else if (idleCount++ > ORPHAN_HEARTBEATS)
            Destroy();
    }

    //public override void OnMoveComplete(WeenieError status)
    //{
    //    base.OnMoveComplete(status);

    //    if (AttackTarget is null && !IsMoving && P_PetOwner is not null)
    //        MoveTo(P_PetOwner);
    //}
}