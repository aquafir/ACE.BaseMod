namespace CustomLoot;

public static class Helpers
{
    public static void SetCloakSpellProc(this WorldObject wo, SpellId spellId)
    {
        if (spellId != SpellId.Undef)
        {
            wo.ProcSpell = (uint)spellId;
            wo.ProcSpellSelfTargeted = spellId.IsSelfTargeting();
            wo.CloakWeaveProc = 1;
        }
        else
        {
            // Damage Reduction proc?
            wo.CloakWeaveProc = 2;
        }
    }

    //Todo: decide whether I need to create an instance of the spell to check?
    //CloakAllId was the original cloak check
    //Aetheria uses a lookup
    public static bool IsSelfTargeting(this SpellId spellId) => new Spell(spellId).IsSelfTargeted; //spellId == SpellId.CloakAllSkill;


    /// <summary>
    /// Assign an EquipmentSetId to a WorldObject based on the loot type
    /// </summary>
    /// <param name="wo"></param>
    public static void RollEquipmentSet(this WorldObject wo, TreasureRoll roll)
    {
        if (!PatchClass.Settings.CustomSets.TryGetValue(roll.ItemType, out var set))
            return; //Remove if missing?

        wo.EquipmentSetId = set[ThreadSafeRandom.Next(0, set.Count - 1)];
    }

    //Find the treasure type from WO.  Not preferred.
    //public static void RollEquipmentSet(this WorldObject wo)
    //{
    //    if (!PatchClass.Settings.CustomSets.TryGetValue(treasureType, out var set))
    //        return; //Remove?

    //    wo.EquipmentSetId = set[ThreadSafeRandom.Next(0, set.Count - 1)];
    //}

    //public static TreasureItemType_Orig GetOriginalTreasureType(this WorldObject wo) => wo switch
    //{
    //    _ when wo.isarm
    //    _ when Cloak.IsCloak(wo) => TreasureItemType_Orig.Cloak,
    //    _ => throw new NotImplementedException(),
    //};




}

public static class RandomExtensions
{
    private static Random random = new Random();

    public static bool TryGetRandom<T>(this T[] array, out T value)
    {
        value = default;

        if (array == null || array.Length == 0)
            return false;

        value = array[random.Next(array.Length)];
        return true;
    }
}