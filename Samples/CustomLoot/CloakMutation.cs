namespace CustomLoot;

public static class CloakMutation
{
    public static void MutateLikeCloak(this WorldObject wo, TreasureDeath profile, TreasureRoll roll)
    {
        wo.ItemMaxLevel = CloakChance.Roll_ItemMaxLevel(profile);
        wo.WieldDifficulty = wo.ItemMaxLevel switch
        {
            1 => 30,
            2 => 60,
            3 => 00,
            4 => 120,
            5 => 150,
            _ => 150,
        };

        wo.IconOverlayId = LootGenerationFactory.IconOverlay_ItemMaxLevel[wo.ItemMaxLevel.Value - 1];

        //Add a set
        //wo.RollEquipmentSet(roll);

        //Use custom set.  Todo: check the target stuff?
        SpellId spellId = PatchClass.Settings.UseCustomCloakSpellProcs ? RollProcSpell() : CloakChance.RollProcSpell();
        wo.SetCloakSpellProc(spellId);

        //Todo, think about these?
        //wo.MaterialType = LootGenerationFactory.GetMaterialType(wo, profile.Tier);
        //wo.Workmanship = WorkmanshipChance.Roll(profile.Tier);
        //if (roll != null && profile.Tier == 8)
        //{
        //    LootGenerationFactory.TryMutateGearRating(wo, profile, roll);
        //}

        //LootGenerationFactory.MutateValue(wo, profile.Tier, roll);
    }

    /// <summary>
    /// Returns spell ID from cloak set
    /// </summary>
    private static SpellId RollProcSpell()
    {
        int num = ThreadSafeRandom.Next(0, PatchClass.Settings.CloakSpells.Count);
        if (num == PatchClass.Settings.CloakSpells.Count)
        {
            return SpellId.Undef;
        }

        return PatchClass.Settings.CloakSpells[num];
    }
}