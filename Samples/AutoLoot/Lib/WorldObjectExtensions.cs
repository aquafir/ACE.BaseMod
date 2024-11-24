namespace AutoLoot.Lib;

public static class WorldObjectExtensions
{
    public static double GetValueDouble(this WorldObject wo, DoubleValueKey key, double defaultValue)
    {
        switch (key)
        {
            case DoubleValueKey.SlashProt:
                return wo.ArmorModVsSlash.HasValue ? wo.ArmorModVsSlash.Value : defaultValue;
            case DoubleValueKey.PierceProt:
                return wo.ArmorModVsPierce.HasValue ? wo.ArmorModVsPierce.Value : defaultValue;
            case DoubleValueKey.BludgeonProt:
                return wo.ArmorModVsBludgeon.HasValue ? wo.ArmorModVsBludgeon.Value : defaultValue;
            case DoubleValueKey.AcidProt:
                return wo.ArmorModVsAcid.HasValue ? wo.ArmorModVsAcid.Value : defaultValue;
            case DoubleValueKey.LightningProt:
                return wo.ArmorModVsElectric.HasValue ? wo.ArmorModVsElectric.Value : defaultValue;
            case DoubleValueKey.FireProt:
                return wo.ArmorModVsFire.HasValue ? wo.ArmorModVsFire.Value : defaultValue;
            case DoubleValueKey.ColdProt:
                return wo.ArmorModVsCold.HasValue ? wo.ArmorModVsCold.Value : defaultValue;
            case DoubleValueKey.ApproachDistance:
                return wo.UseRadius.HasValue ? wo.UseRadius.Value : defaultValue;
            case DoubleValueKey.SalvageWorkmanship:
                return wo.Workmanship.HasValue ? wo.Workmanship.Value : defaultValue;
            case DoubleValueKey.Scale:
                return wo.ObjScale.HasValue ? wo.ObjScale.Value : defaultValue;
            case DoubleValueKey.Variance:
                return wo.DamageVariance.HasValue ? wo.DamageVariance.Value : defaultValue;
            case DoubleValueKey.DamageBonus:
                return wo.DamageMod.HasValue ? wo.DamageMod.Value : defaultValue;
            default:
                return wo.GetProperty((PropertyFloat)key) ?? defaultValue;
        }
    }
    public static bool GetValueBool(this WorldObject wo, BoolValueKey key, bool defaultValue)
    {
        switch (key)
        {
            case BoolValueKey.Inscribable:
                return wo.Inscribable;
            case BoolValueKey.Lockable:
                return wo.DefaultLocked || wo.IsLocked;
            default:
                return wo.GetProperty((PropertyBool)key) ?? defaultValue;
        }
    }

    public static string GetValueString(this WorldObject wo, StringValueKey key, string defaultValue)
    {
        switch (key)
        {
            case StringValueKey.SecondaryName:
                return wo.PluralName;
            default:
                return wo.GetProperty((PropertyString)key) ?? defaultValue;
        }
    }

    public static int GetValueInt(this WorldObject wo, IntValueKey key, int defaultValue)
    {
        switch (key)
        {
            case IntValueKey.Type:
                return (int)wo.WeenieType;
            case IntValueKey.Icon:
                return (int)wo.IconId;
            case IntValueKey.Container:
                return wo.ContainerId.HasValue ? (int)wo.ContainerId.Value : defaultValue;
            case IntValueKey.Landblock:
                return (int)wo.CurrentLandblock.Id.Raw;
            case IntValueKey.ItemSlots:
                return wo.ItemCapacity.HasValue ? wo.ItemCapacity.Value : defaultValue;
            case IntValueKey.PackSlots:
                return wo.ContainerCapacity.HasValue ? wo.ContainerCapacity.Value : defaultValue;
            case IntValueKey.StackCount:
                return wo.StackSize.HasValue ? wo.StackSize.Value : defaultValue;
            case IntValueKey.StackMax:
                return wo.MaxStackSize.HasValue ? wo.MaxStackSize.Value : defaultValue;
            case IntValueKey.AssociatedSpell:
                return wo.SpellDID.HasValue ? (int)wo.SpellDID.Value : defaultValue;
            case IntValueKey.Wielder:
                return wo.WielderId.HasValue ? (int)wo.WielderId.Value : defaultValue;
            case IntValueKey.WieldingSlot:
            case IntValueKey.Slot:
                return wo.CurrentWieldedLocation.HasValue ? (int)wo.CurrentWieldedLocation.Value : defaultValue;
            case IntValueKey.Monarch:
                return wo.MonarchId.HasValue ? (int)wo.MonarchId.Value : defaultValue;
            case IntValueKey.EquipableSlots:
            case IntValueKey.Coverage:
                return wo.ValidLocations.HasValue ? (int)wo.ValidLocations.Value : defaultValue;
            case IntValueKey.IconOutline:
                return wo.UiEffects.HasValue ? (int)wo.UiEffects.Value : defaultValue;
            case IntValueKey.UsageMask:
                return wo.ItemUseable.HasValue ? (int)wo.ItemUseable.Value : defaultValue;
            case IntValueKey.HouseOwner:
                return wo.HouseOwner.HasValue ? (int)wo.HouseOwner.Value : defaultValue;
            case IntValueKey.HookMask:
                return wo.HookItemType.HasValue ? wo.HookItemType.Value : defaultValue;
            case IntValueKey.HookType:
                return wo.HookType.HasValue ? wo.HookType.Value : defaultValue;
            case IntValueKey.Model:
                return (int)wo.SetupTableId;
            case IntValueKey.Flags:
                return (int)wo.ObjectDescriptionFlags;
            case IntValueKey.CreateFlags1:
                return (int)wo.CalculateWeenieHeaderFlag();
            case IntValueKey.CreateFlags2:
                return (int)wo.CalculateWeenieHeaderFlag2();
            case IntValueKey.Category:
                return (int)wo.ItemType;
            case IntValueKey.Behavior:
                return wo.RadarBehavior.HasValue ? (int)wo.RadarBehavior.Value : defaultValue;
            case IntValueKey.MagicDef:
                return wo.WeaponMagicDefense.HasValue ? (int)wo.WeaponMagicDefense.Value : defaultValue;
            case IntValueKey.SpellCount:
                return wo.Biota.PropertiesSpellBook.Count;
            case IntValueKey.WeapSpeed:
                return wo.WeaponTime.HasValue ? wo.WeaponTime.Value : defaultValue;
            case IntValueKey.EquipSkill:
                return wo.UseRequiresSkillLevel.HasValue ? wo.UseRequiresSkillLevel.Value : defaultValue;
            case IntValueKey.DamageType:
                return (int)wo.W_DamageType;
            case IntValueKey.MaxDamage:
                return wo.Damage.HasValue ? wo.Damage.Value : defaultValue;
            case IntValueKey.ItemUsabilityFlags:
                return wo.ItemUseable.HasValue ? (int)wo.ItemUseable.Value : defaultValue;
            case IntValueKey.PhysicsDataFlags:
                return (int)wo.CalculatedPhysicsDescriptionFlag();
            case IntValueKey.ActiveSpellCount:
                return wo.GetActiveSpellCount();
            case IntValueKey.IconOverlay:
                return wo.IconOverlayId.HasValue ? (int)wo.IconOverlayId.Value : defaultValue;
            case IntValueKey.IconUnderlay:
                return wo.IconUnderlayId.HasValue ? (int)wo.IconUnderlayId.Value : defaultValue;
            case IntValueKey.EquippedBy:
                return wo.WielderId.HasValue ? (int)wo.WielderId.Value : defaultValue;
            case IntValueKey.LastAttacker:
                return wo.CurrentAttacker.HasValue ? (int)wo.CurrentAttacker.Value : defaultValue;
            case IntValueKey.Patron:
                return wo.PatronId.HasValue ? (int)wo.PatronId.Value : defaultValue;
            default:
                return wo.GetProperty((PropertyInt)key) ?? defaultValue;
        }
    }

    public static bool IsMagical(this WorldObject wo)
    {
        return (wo.UiEffects & UiEffects.Magical) != 0;
    }

    public static List<Spell> GetSpells(this WorldObject wo)
    {
        return wo.Biota?.PropertiesSpellBook?.Keys.ToList().Select(s => new Spell(s)).ToList() ?? new List<Spell>();
    }

    public static int GetActiveSpellCount(this WorldObject wo)
    {
        return wo.EnchantmentManager.GetEnchantments(MagicSchool.LifeMagic).Count + wo.EnchantmentManager.GetEnchantments(MagicSchool.CreatureEnchantment).Count + wo.EnchantmentManager.GetEnchantments(MagicSchool.ItemEnchantment).Count;
    }

    public static ObjectClass GetObjectClass(this WorldObject wo)
    {
        ObjectClass objectClass = ObjectClass.Unknown;
        int _type = (int)wo.ItemType;
        int _bools = (int)wo.ObjectDescriptionFlags;
        int num3 = (int)wo.CalculateWeenieHeaderFlag();

        if ((_type & 1) > 0) objectClass = ObjectClass.MeleeWeapon;
        else if ((_type & 2) > 0) objectClass = ObjectClass.Armor;
        else if ((_type & 4) > 0) objectClass = ObjectClass.Clothing;
        else if ((_type & 8) > 0) objectClass = ObjectClass.Jewelry;
        else if ((_type & 16) > 0) objectClass = ObjectClass.Monster;
        else if ((_type & 32) > 0) objectClass = ObjectClass.Food;
        else if ((_type & 64) > 0) objectClass = ObjectClass.Money;
        else if ((_type & 128) > 0) objectClass = ObjectClass.Misc;
        else if ((_type & 256) > 0) objectClass = ObjectClass.MissileWeapon;
        else if ((_type & 512) > 0) objectClass = ObjectClass.Container;
        else if ((_type & 1024) > 0) objectClass = ObjectClass.Bundle;
        else if ((_type & 2048) > 0) objectClass = ObjectClass.Gem;
        else if ((_type & 4096) > 0) objectClass = ObjectClass.SpellComponent;
        else if ((_type & 16384) > 0) objectClass = ObjectClass.Key;
        else if ((_type & 32768) > 0) objectClass = ObjectClass.WandStaffOrb;
        else if ((_type & 65536) > 0) objectClass = ObjectClass.Portal;
        else if ((_type & 262144) > 0) objectClass = ObjectClass.TradeNote;
        else if ((_type & 524288) > 0) objectClass = ObjectClass.ManaStone;
        else if ((_type & 1048576) > 0) objectClass = ObjectClass.Services;
        else if ((_type & 2097152) > 0) objectClass = ObjectClass.Plant;
        else if ((_type & 4194304) > 0) objectClass = ObjectClass.BaseCooking;
        else if ((_type & 8388608) > 0) objectClass = ObjectClass.BaseAlchemy;
        else if ((_type & 16777216) > 0) objectClass = ObjectClass.BaseFletching;
        else if ((_type & 33554432) > 0) objectClass = ObjectClass.CraftedCooking;
        else if ((_type & 67108864) > 0) objectClass = ObjectClass.CraftedAlchemy;
        else if ((_type & 134217728) > 0) objectClass = ObjectClass.CraftedFletching;
        else if ((_type & 536870912) > 0) objectClass = ObjectClass.Ust;
        else if ((_type & 1073741824) > 0) objectClass = ObjectClass.Salvage;
        if ((_bools & 8) > 0) objectClass = ObjectClass.Player;
        else if ((_bools & 512) > 0) objectClass = ObjectClass.Vendor;
        else if ((_bools & 4096) > 0) objectClass = ObjectClass.Door;
        else if ((_bools & 8192) > 0) objectClass = ObjectClass.Corpse;
        else if ((_bools & 16384) > 0) objectClass = ObjectClass.Lifestone;
        else if ((_bools & 32768) > 0) objectClass = ObjectClass.Food;
        else if ((_bools & 65536) > 0) objectClass = ObjectClass.HealingKit;
        else if ((_bools & 131072) > 0) objectClass = ObjectClass.Lockpick;
        else if ((_bools & 262144) > 0) objectClass = ObjectClass.Portal;
        else if ((_bools & 8388608) > 0) objectClass = ObjectClass.Foci;
        else if ((_bools & 1) > 0) objectClass = ObjectClass.Container;
        if ((_type & 8192) > 0 && (_bools & 256) > 0 && objectClass == ObjectClass.Unknown)
        {
            if ((_bools & 2) > 0) objectClass = ObjectClass.Journal;
            else if ((_bools & 4) > 0) objectClass = ObjectClass.Sign;
            else if ((_bools & 15) > 0) objectClass = ObjectClass.Book;
        }
        if ((_type & 8192) > 0 && (num3 & 4194304) > 0) objectClass = ObjectClass.Scroll;
        if (objectClass == ObjectClass.Monster && (_bools & 16) == 0) objectClass = ObjectClass.Npc;
        if (objectClass == ObjectClass.Monster && (_bools & 67108864) != 0) objectClass = ObjectClass.Npc;

        return objectClass;
    }
}
