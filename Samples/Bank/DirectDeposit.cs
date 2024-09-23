using static ACE.Server.WorldObjects.Player;

namespace Bank;
[HarmonyPatchCategory(nameof(DirectDeposit))]
public class DirectDeposit
{
    //Command to opt out
    [CommandHandler("ddt", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleBank(Session session, params string[] parameters)
    {
        var player = session.Player;

        var dd = player.GetProperty(FakeBool.BankUsesDirectDeposit) ?? true;

        player.SetProperty(FakeBool.BankUsesDirectDeposit, !dd);

        player.SendMessage($"You are using {(dd ? "no longer" : "now")} direct deposit.");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionSellItem), new Type[] { typeof(uint), typeof(List<ItemProfile>) })]
    public static bool PreHandleActionSellItem(uint vendorGuid, List<ItemProfile> itemProfiles, ref Player __instance)
    {
        if (__instance is null || __instance.GetProperty(FakeBool.BankUsesDirectDeposit) == false)
            return true;

        if (__instance.IsBusy)
        {
            __instance.SendUseDoneEvent(WeenieError.YoureTooBusy);
            return false;
        }

        var vendor = __instance.CurrentLandblock?.GetObject(vendorGuid) as Vendor;

        if (vendor == null)
        {
            __instance.SendUseDoneEvent(WeenieError.NoObject);
            return false;
        }

        // perform validations on requested sell items,
        // and filter to list of validated items

        // one difference between sell and buy is here.
        // when an itemProfile is invalid in buy, the entire transaction is failed immediately.
        // when an itemProfile is invalid in sell, we just remove the invalid itemProfiles, and continue onwards
        // this might not be the best for safety, and it's a tradeoff between safety and player convenience
        // should we fail the entire transaction (similar to buy), if there are any invalids in the transaction request?

        var sellList = __instance.VerifySellItems(itemProfiles, vendor);

        if (sellList.Count == 0)
        {
            __instance.Session.Network.EnqueueSend(new GameEventInventoryServerSaveFailed(__instance.Session, __instance.Guid.Full));
            __instance.SendUseDoneEvent();
            return false;
        }

        // calculate pyreals to receive
        var payoutCoinAmount = vendor.CalculatePayoutCoinAmount(sellList);

        //Validate
        if (payoutCoinAmount < 0)
        {
            ModManager.Log($"[VENDOR] {__instance.Name} (0x({__instance.Guid}) tried to sell something to {vendor.Name} (0x{vendor.Guid}) resulting in a payout of {payoutCoinAmount} pyreals.");

            __instance.SendTransientError("Transaction failed.");
            __instance.Session.Network.EnqueueSend(new GameEventInventoryServerSaveFailed(__instance.Session, __instance.Guid.Full));

            __instance.SendUseDoneEvent();
            return false;
        }

        //Increase of adding pyreal stacks add amount directly
        __instance.IncCash(payoutCoinAmount);
        __instance.SendMessage($"Deposited {payoutCoinAmount:N0}.  Balance is {__instance.GetCash():N0}");

        vendor.MoneyOutflow += payoutCoinAmount;

        // remove sell items from player inventory
        foreach (var item in sellList.Values)
        {
            if (__instance.TryRemoveFromInventoryWithNetworking(item.Guid, out _, RemoveFromInventoryAction.SellItem) || __instance.TryDequipObjectWithNetworking(item.Guid, out _, DequipObjectAction.SellItem))
                __instance.Session.Network.EnqueueSend(new GameEventItemServerSaysContainId(__instance.Session, item, vendor));
        }

        // send the list of items to the vendor
        // for the vendor to determine what to do with each item (resell, destroy)
        vendor.ProcessItemsForPurchase(__instance, sellList);

        __instance.Session.Network.EnqueueSend(new GameMessageSound(__instance.Guid, Sound.PickUpItem));
        __instance.SendUseDoneEvent();

        return false;
    }

}
