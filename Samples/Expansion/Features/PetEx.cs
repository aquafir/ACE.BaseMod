
using ACE.Server.Realms;
using System;

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

    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (P_PetOwner is not null && (AttackTarget is null || AttackTarget.IsDestroyed))
        {
            if (P_PetOwner.GetCylinderDistance(this) > 60)
            {
                //Todo: ask RF about FakeTeleport
                FakeTeleport(P_PetOwner.Location.InFrontOf(1));
                P_PetOwner.SendMessage($"{Name} has caught up to you.");
            }
            else if(!IsMoving)
                MoveTo(P_PetOwner);
        }
    }

    //public override void OnMoveComplete(WeenieError status)
    //{
    //    base.OnMoveComplete(status);

    //    if (AttackTarget is null && !IsMoving && P_PetOwner is not null)
    //        MoveTo(P_PetOwner);
    //}
}