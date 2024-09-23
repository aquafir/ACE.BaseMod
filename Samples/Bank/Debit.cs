using ACE.Database;
using ACE.Server.Network.GameEvent;
using static ACE.Server.WorldObjects.Player;

namespace Bank;

/// <summary>
/// Overrides purchase w/pyreals and alt currency
/// </summary>
[HarmonyPatchCategory(nameof(Debit))]
public class Debit
{

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateCoinValue), new Type[] { typeof(bool) })]
    public static bool PreUpdateCoinValue(bool sendUpdateMessageIfChanged, ref Player __instance)
    {
        long banked = __instance.GetCash();

        foreach (var coinStack in __instance.GetInventoryItemsOfTypeWeenieType(WeenieType.Coin))
            banked += coinStack.Value ?? 0;

        //Cap at max int
        int coins = banked > int.MaxValue ? int.MaxValue : (int)banked;

        if (sendUpdateMessageIfChanged && __instance.CoinValue == coins)
            sendUpdateMessageIfChanged = false;

        __instance.CoinValue = coins;

        if (sendUpdateMessageIfChanged)
            __instance.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(__instance, PropertyInt.CoinValue, __instance.CoinValue ?? 0));
        return false;
    }



    //Send the player the held and banked currency/alt coins
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameEventApproachVendor), MethodType.Constructor, new Type[] { typeof(Session), typeof(Vendor), typeof(uint) })]
    public static bool PreCtorGameEventApproachVendor(Session session, Vendor vendor, uint altCurrencySpent, ref GameEventApproachVendor __instance)
    {
        //Workaround initialization since base is awkward..?
        __instance.Base(session, GameEventType.ApproachVendor, GameMessageGroup.UIQueue);




        __instance.Writer.Write(vendor.Guid.Full);

        // the types of items vendor will purchase
        __instance.Writer.Write((uint)vendor.MerchandiseItemTypes);
        __instance.Writer.Write((uint)vendor.MerchandiseMinValue);
        __instance.Writer.Write((uint)vendor.MerchandiseMaxValue);

        __instance.Writer.Write(Convert.ToUInt32(vendor.DealMagicalItems ?? false));

        __instance.Writer.Write((float)vendor.BuyPrice);
        __instance.Writer.Write((float)vendor.SellPrice);

        // the wcid of the alternate currency
        __instance.Writer.Write(vendor.AlternateCurrency ?? 0);

        // if this vendor accepts items as alternate currency, instead of pyreals
        if (vendor.AlternateCurrency != null)
        {
            var altCurrency = DatabaseManager.World.GetCachedWeenie(vendor.AlternateCurrency.Value);
            var pluralName = altCurrency.GetPluralName();

            // the total amount of alternate currency the player currently has
            var altCurrencyInInventory = (uint)session.Player.GetNumInventoryItemsOfWCID(vendor.AlternateCurrency.Value);

            //!!ADD BANKED AMOUNT
            var banked = PatchClass.Settings.Items.Where(x => x.Id == vendor.AlternateCurrency.Value).FirstOrDefault();
            if (banked is not null)
                altCurrencyInInventory += (uint)session.Player.GetBanked(banked.Prop);

            __instance.Writer.Write(altCurrencyInInventory + altCurrencySpent);

            // the plural name of alt currency
            __instance.Writer.WriteString16L(pluralName);
        }
        else
        {
            //Original
            __instance.Writer.Write(0);
            __instance.Writer.WriteString16L(string.Empty);
        }

        var numItems = vendor.DefaultItemsForSale.Count + vendor.UniqueItemsForSale.Count;

        __instance.Writer.Write(numItems);


        //!!Passed action crashed
        //vendor.forEachItem((obj) =>
        //{
        //    int stackSize = obj.VendorShopCreateListStackSize ?? obj.StackSize ?? 1; // -1 = unlimited supply
        //    vend.Writer.Write(stackSize & 0xFFFFFF | -1 << 24);
        //    obj.SerializeGameDataOnly(vend.Writer);
        //});
        //For each default/unique item write the stack size
        foreach (var item in vendor.DefaultItemsForSale.Values)
        {
            int stackSize = item.VendorShopCreateListStackSize ?? item.StackSize ?? 1; // -1 = unlimited supply
            __instance.Writer.Write(stackSize & 0xFFFFFF | -1 << 24);
            item.SerializeGameDataOnly(__instance.Writer);
        }
        foreach (var item in vendor.UniqueItemsForSale.Values)
        {
            int stackSize = item.VendorShopCreateListStackSize ?? item.StackSize ?? 1; // -1 = unlimited supply
            __instance.Writer.Write(stackSize & 0xFFFFFF | -1 << 24);
            item.SerializeGameDataOnly(__instance.Writer);
        }

        __instance.Writer.Align();

        return false;
    }

    //Rework spend currency to take from the bank first
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.SpendCurrency), new Type[] { typeof(uint), typeof(uint), typeof(bool) })]
    public static bool PreSpendCurrency(uint currentWcid, uint amount, bool destroy, ref Player __instance, ref List<WorldObject> __result)
    {
        if (currentWcid == 0 || amount == 0)
        {
            __result = null;
            return false;
        }


        //Take from bank first
        if (currentWcid == coinStackWcid)
        {
            var cash = __instance.GetCash();

            //Use up to the amount requested
            var used = (int)Math.Min(cash, amount);
            __instance.IncCash(-used);

            __instance.SendMessage($"Used {used:N0} pyreals from bank of the {amount:N0} needed.  {cash - used:N0} remaining.");
            amount -= (uint)used;

        }
        else
        {
            var bankEntry = PatchClass.Settings.Items.Where(x => x.Id == currentWcid).FirstOrDefault();
            if (bankEntry is not null)
            {
                //Get amount banked
                var banked = __instance.GetBanked(bankEntry.Prop);

                //Use up to the amount requested
                var used = (int)Math.Min(banked, amount);
                __instance.IncBanked(bankEntry.Prop, -used);

                __instance.SendMessage($"Used {used:N0} {bankEntry.Name} from bank of the {amount} needed.  {banked - used:N0} remaining.");
                amount -= (uint)used;
            }
        }



        var cost = new List<WorldObject>();

        if (currentWcid == coinStackWcid)
        {
            if (amount > __instance.CoinValue)
            {
                __result = null;
                return false;
            }
        }
        if (destroy)
        {
            __instance.TryConsumeFromInventoryWithNetworking(currentWcid, (int)amount);
        }
        else
        {
            cost = __instance.CollectCurrencyStacks(currentWcid, amount);

            foreach (var stack in cost)
            {
                if (!__instance.TryRemoveFromInventoryWithNetworking(stack.Guid, out _, RemoveFromInventoryAction.SpendItem))
                    __instance.UpdateCoinValue(); // this coinstack was created by spliting up an existing one, and not actually added to the players inventory. The existing stack was already adjusted down but we need to update the player's CoinValue, so we do that now.
            }
        }

        __result = cost;
        return false;
    }

    //Player finalizes buy
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.FinalizeBuyTransaction), new Type[] { typeof(Vendor), typeof(List<WorldObject>), typeof(List<WorldObject>), typeof(uint) })]
    public static bool PreFinalizeBuyTransaction(Vendor vendor, List<WorldObject> genericItems, List<WorldObject> uniqueItems, uint cost, ref Player __instance)
    {
        // transaction has been validated by this point

        var currencyWcid = vendor.AlternateCurrency ?? coinStackWcid;

        __instance.SpendCurrency(currencyWcid, cost, true);

        vendor.MoneyIncome += (int)cost;

        foreach (var item in genericItems)
        {
            var service = item.GetProperty(PropertyBool.VendorService) ?? false;

            if (!service)
            {
                // errors shouldn't be possible here, since the items were pre-validated, but just in case...
                if (!__instance.TryCreateInInventoryWithNetworking(item))
                {
                    //Player.log.Error($"[VENDOR] {__instance.Name}.FinalizeBuyTransaction({vendor.Name}) - couldn't add {item.Name} ({item.Guid}) to player inventory after validation, this shouldn't happen!");

                    item.Destroy();  // cleanup for guid manager
                }

                vendor.NumItemsSold++;
            }
            else
                vendor.ApplyService(item, __instance);
        }

        foreach (var item in uniqueItems)
        {
            if (__instance.TryCreateInInventoryWithNetworking(item))
            {
                vendor.UniqueItemsForSale.Remove(item.Guid);

                // this was only for when the unique item was sold to the vendor,
                // to determine when the item should rot on the vendor. it gets removed now
                item.SoldTimestamp = null;

                vendor.NumItemsSold++;
            }
            else { }
            //log.Error($"[VENDOR] {Name}.FinalizeBuyTransaction({vendor.Name}) - couldn't add {item.Name} ({item.Guid}) to player inventory after validation, this shouldn't happen!");
        }

        __instance.Session.Network.EnqueueSend(new GameMessageSound(__instance.Guid, Sound.PickUpItem));

        if (PropertyManager.GetBool("player_receive_immediate_save").Item)
            __instance.RushNextPlayerSave(5);

        var altCurrencySpent = vendor.AlternateCurrency != null ? cost : 0;

        vendor.ApproachVendor(__instance, VendorType.Buy, altCurrencySpent);

        return false;
    }

    //Check that the player has enough currency
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vendor), nameof(Vendor.BuyItems_ValidateTransaction), new Type[] { typeof(List<ItemProfile>), typeof(Player) })]
    public static bool PreBuyItems_ValidateTransaction(List<ItemProfile> itemProfiles, Player player, ref Vendor __instance, ref bool __result)
    {
        // one difference between buy and sell currently
        // is that if *any* items in the buy transactions are detected as invalid,
        // we reject the entire transaction.
        // this seems to be the "safest" route, however in terms of player convenience
        // where only 1 item has an error from a large purchase set,
        // this might not be the most convenient for the player.

        var defaultItemProfiles = new List<ItemProfile>();
        var uniqueItems = new List<WorldObject>();

        // find item profiles in default and unique items
        foreach (var itemProfile in itemProfiles)
        {
            if (!itemProfile.IsValidAmount)
            {
                // reject entire transaction immediately
                player.SendTransientError($"Invalid amount");
                __result = false;
                return false;
            }

            var itemGuid = new ObjectGuid(itemProfile.ObjectGuid);

            // check default items
            if (__instance.DefaultItemsForSale.TryGetValue(itemGuid, out var defaultItemForSale))
            {
                itemProfile.WeenieClassId = defaultItemForSale.WeenieClassId;
                itemProfile.Palette = defaultItemForSale.PaletteTemplate;
                itemProfile.Shade = defaultItemForSale.Shade;

                defaultItemProfiles.Add(itemProfile);
            }
            // check unique items
            else if (__instance.UniqueItemsForSale.TryGetValue(itemGuid, out var uniqueItemForSale))
            {
                uniqueItems.Add(uniqueItemForSale);
            }
        }

        // ensure player has enough free inventory slots / container slots / available burden to receive items
        var itemsToReceive = new ItemsToReceive(player);

        foreach (var defaultItemProfile in defaultItemProfiles)
        {
            itemsToReceive.Add(defaultItemProfile.WeenieClassId, defaultItemProfile.Amount);

            if (itemsToReceive.PlayerExceedsLimits)
                break;
        }

        if (!itemsToReceive.PlayerExceedsLimits)
        {
            foreach (var uniqueItem in uniqueItems)
            {
                itemsToReceive.Add(uniqueItem.WeenieClassId, uniqueItem.StackSize ?? 1);

                if (itemsToReceive.PlayerExceedsLimits)
                    break;
            }
        }

        if (itemsToReceive.PlayerExceedsLimits)
        {
            if (itemsToReceive.PlayerExceedsAvailableBurden)
                player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "You are too encumbered to buy that!"));
            else if (itemsToReceive.PlayerOutOfInventorySlots)
                player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "You do not have enough pack space to buy that!"));
            else if (itemsToReceive.PlayerOutOfContainerSlots)
                player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "You do not have enough container slots to buy that!"));

            __result = false;
            return false;
        }

        // ideally the creation of the wo's would be delayed even further,
        // and all validations would be performed on weenies beforehand
        // this would require:
        // - a forEach helper function to iterate through both defaultItemProfiles (ItemProfiles) and uniqueItems (WorldObjects),
        //   so that 2 foreach iterators don't have to be written each time
        // - weenie to have more functions that mimic the functionality of WorldObject

        // create world objects for default items
        var defaultItems = new List<WorldObject>();

        foreach (var defaultItemProfile in defaultItemProfiles)
            defaultItems.AddRange(__instance.ItemProfileToWorldObjects(defaultItemProfile));

        var purchaseItems = defaultItems.Concat(uniqueItems).ToList();

        if (__instance.IsBusy && purchaseItems.Any(i => i.GetProperty(PropertyBool.VendorService) == true))
        {
            player.SendWeenieErrorWithString(WeenieErrorWithString._IsTooBusyToAcceptGifts, __instance.Name);
            Vendor.CleanupCreatedItems(defaultItems);
            __result = false;
            return false;
        }

        // check uniques
        if (!player.CheckUniques(purchaseItems, __instance))
        {
            Vendor.CleanupCreatedItems(defaultItems);
            __result = false;
            return false;
        }

        // calculate price
        uint totalPrice = 0;

        foreach (var item in purchaseItems)
        {
            var cost = __instance.GetSellCost(item);

            // detect rollover?
            totalPrice += cost;
        }

        // verify player has enough currency
        if (__instance.AlternateCurrency == null)
        {
            //Add in player cash
            var cash = player.GetCash();
            if (cash + player.CoinValue < totalPrice)
            {
                Vendor.CleanupCreatedItems(defaultItems);
                __result = false;
                return false;
            }
        }
        else
        {
            var playerAltCurrency = player.GetNumInventoryItemsOfWCID(__instance.AlternateCurrency.Value);

            //!!ADD BANKED AMOUNT
            var wcid = __instance.AlternateCurrency.Value;
            var banked = PatchClass.Settings.Items.Where(x => x.Id == wcid).FirstOrDefault();
            if (banked is not null)
                playerAltCurrency += (int)player.GetBanked(banked.Prop);


            if (playerAltCurrency < totalPrice)
            {
                Vendor.CleanupCreatedItems(defaultItems);
                __result = false;
                return false;
            }
        }

        // everything is verified at this point

        // send transaction to player for further processing
        player.FinalizeBuyTransaction(__instance, defaultItems, uniqueItems, totalPrice);

        __result = true;
        return false;
    }
}
