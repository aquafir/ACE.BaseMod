namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.ItemLevelUpGrowth))]
public static class ItemLevelUpGrowth
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnItemLevelUp), new Type[] { typeof(WorldObject), typeof(int) })]
    public static bool PreOnItemLevelUp(WorldObject item, int prevItemLevel, ref Player __instance)
    {
        //Skip items not made as growth items
        if (item.GetProperty(FakeBool.GrowthItem) is null)  //Will this ever by false?
            return false;

        for (int i = prevItemLevel; i < item.ItemLevel; i++)
        {
            __instance.SendMessage($"Growing {item.Name} - {item.ItemType} - {i}");

            switch (item.ItemType)
            {
                case ItemType.MissileWeapon:
                case ItemType.Weapon:
                case ItemType.WeaponOrCaster:
                case ItemType.MeleeWeapon:
                    GrowWeapon(item, __instance,i);
                    break;
                case ItemType.Armor:
                    GrowArmor(item, __instance,i);
                    break;
                case ItemType.Clothing:
                    break;
                case ItemType.Jewelry:
                    break;
            }
        }

        //Debugger.Break();
        //if (item.ImbuedEffect != ImbuedEffectType.Undef)
        //    return true;

        //if (ImbueGroup.AllRending.SetOf().TryGetRandom(out var imbuedEffectType))
        //    item.ImbuedEffect = imbuedEffectType;

        //Return true to execute original
        return true;
    }

    public static void GrowWeapon(this WorldObject item, Player player, int level)
    {
        switch (level)
        {
            //Try to imbue on first level
            case 1:
                if (ImbueGroup.AllRending.SetOf().TryGetRandom(out var imbuedEffectType))
                {
                    item.ImbuedEffect = imbuedEffectType;
                    item.IconUnderlayId = 0x06005B0C;   //Rares //RecipeManager.IconUnderlay[imbuedEffectType];
                    item.UiEffects = UiEffects.BoostHealth;
                    player.UpdateProperty(item, PropertyDataId.IconUnderlay, 0x06005B0C, true);

                    player.SendMessage($"Your {item.Name} was imbued with {imbuedEffectType}");
                }
                break;

            default:
                item.WeaponOffense += .02;
                item.WeaponDefense += .02;
                item.Damage += 1;

                player.SendMessage($"Your {item.Name} got 2% to skill.");
                break;
        }
    }

    public static void GrowArmor(this WorldObject item, Player player, int level)
    {
        switch (level)
        {
            //Try to imbue on first level
            //case 1:
            //    if (ImbueGroup.AllRending.SetOf().TryGetRandom(out var imbuedEffectType))
            //    {
            //        item.ImbuedEffect = imbuedEffectType;

            //        player.SendMessage($"Your {item.Name} was imbued with {imbuedEffectType}");
            //    }
            //    break;

            default:
                item.ArmorLevel += 20;

                player.SendMessage($"Your {item.Name} got 20 armor?");
                break;
        }
    }
}
