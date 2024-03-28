namespace Expansion.Mutators;

public class ProcOnAttack : Mutator
{
    #region Todo: Move somewhere 
    public static Dictionary<SpellId, bool> SurgeTargetSelf = new Dictionary<SpellId, bool>()
        {
            { SpellId.AetheriaProcDamageBoost,     true },
            { SpellId.AetheriaProcDamageOverTime,  false },
            { SpellId.AetheriaProcDamageReduction, true },
            { SpellId.AetheriaProcHealDebuff,      false },
            { SpellId.AetheriaProcHealthOverTime,  true },
        };

    public static Dictionary<Sigil, EquipmentSet> SigilToEquipmentSet = new Dictionary<Sigil, EquipmentSet>()
        {
            { Sigil.Defense, EquipmentSet.AetheriaDefense },
            { Sigil.Destruction, EquipmentSet.AetheriaDestruction },
            { Sigil.Fury, EquipmentSet.AetheriaFury },
            { Sigil.Growth, EquipmentSet.AetheriaGrowth },
            { Sigil.Vigor, EquipmentSet.AetheriaVigor }
        };
    #endregion

    public override bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath treasureDeath, TreasureRoll treasureRoll, WorldObject item)
    {
        //Todo
        //throw new NotImplementedException();
        return false;
    }
    public static void ActivateSigil(WorldObject target)
    {
        // rng select a sigil / spell set
        var randSigil = (Sigil)ThreadSafeRandom.Next(0, 4);

        var equipmentSet = SigilToEquipmentSet[randSigil];
        target.SetProperty(PropertyInt.EquipmentSetId, (int)equipmentSet);

        // change icon
        //var color = GetColor(target.WeenieClassId).Value;
        //var icon = Icons[color][randSigil];
        //target.SetProperty(PropertyDataId.Icon, icon);

        // rng select a surge spell
        var surgeSpell = (SpellId)ThreadSafeRandom.Next(5204, 5208);

        target.SetProperty(PropertyDataId.ProcSpell, (uint)surgeSpell);

        //if (Sets.SurgeTargetSelf[surgeSpell])
        if (new Spell(surgeSpell).IsSelfTargeted)
            target.SetProperty(PropertyBool.ProcSpellSelfTargeted, true);

        // set equip mask
        //target.SetProperty(PropertyInt.ValidLocations, (int)ColorToMask[color]);
    }

    //Mutating at lootgen level so pass profiles
    public static void MutateLikeAetheria(WorldObject wo, TreasureDeath profile, TreasureRoll roll)//int tier)
    {
        //Mutate pre-activated 
        // Initial roll for an Aetheria level 1 through 3
        wo.ItemMaxLevel = 1;

        //Messy max level logic
        wo.ItemMaxLevel = ThreadSafeRandom.Next(1, 7) switch
        {
            var x when x > 6 => 3,
            var x when x > 4 => 2,
            _ => 1,
        };

        // Perform an additional roll check for a chance at a higher Aetheria level for tiers 6+
        if (profile.Tier > 5)
        {
            if (ThreadSafeRandom.Next(1, 50) == 1)
            {
                wo.ItemMaxLevel = 4;
                if (profile.Tier > 6 && ThreadSafeRandom.Next(1, 5) == 1)
                {
                    wo.ItemMaxLevel = 5;
                }
            }
        }
        wo.IconOverlayId = LootGenerationFactory.IconOverlay_ItemMaxLevel[wo.ItemMaxLevel.Value - 1];

        //Activate
        ActivateSigil(wo);
    }

    //private static SpellId RollProcSpell()
    //{
    //    int num = ThreadSafeRandom.Next(0, PatchClass.Settings.ProcOnHitSpells.Count);
    //    if (num == PatchClass.Settings.ProcOnHitSpells.Count)
    //    {
    //        return SpellId.Undef;
    //    }

    //    return PatchClass.Settings.ProcOnHitSpells[num];
    //}
}
