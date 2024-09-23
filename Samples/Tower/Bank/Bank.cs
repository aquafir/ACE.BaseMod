namespace Tower;

[CommandCategory(nameof(Feature.Bank))]
[HarmonyPatchCategory(nameof(Feature.Bank))]
public static class Bank
{
    static BankSettings Settings => PatchClass.Settings.Bank;

    static string Commands => string.Join(", ", Enum.GetNames<Transaction>());

    [CommandHandler("bank", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleBank(Session session, params string[] parameters)
    {
        var player = session.Player;

        //Try to parse a valid command
        if (!parameters.TryParseCommand(out var verb, out var name, out var amount, out var wildcardAmount, out var recipient))
        {
            player.SendMessage($"Usage: <command> [name|id [amount=1]]\nAvailable commands: {Commands}");
            return;
        }
        //player.SendMessage($"{verb} - {name} - {amount} - {wildcardAmount} - {recipient}");

        if (verb == Transaction.List)
        {
            HandleList(player);
            return;
        }

        //Parse information needed for other commands
        if (string.IsNullOrWhiteSpace(name))
        {
            player.SendMessage($"Specify the name or WCID of the item to transact with.");
            return;
        }

        //Try to parse weenie
        var query = int.TryParse(name, out var wcid) ?
            Settings.Items.Where(x => x.Id == wcid) :
            Settings.Items.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        var item = query.FirstOrDefault();
        if (item is null)
        {
            player.SendMessage($"Unable to find matching item: {name}");
            return;
        }

        //Take the cap if it's smaller
        if (wildcardAmount || Settings.ExcessSetToMax)
        {
            var held = verb == Transaction.Give ? player.GetNumInventoryItemsOfWCID(item.Id) : (int)player.GetBanked(item.Prop);
            amount = Math.Min(amount, held);
        }

        switch (verb)
        {
            case Transaction.Give:
                TryHandleDeposit(player, item, amount);
                break;
            case Transaction.Take:
                TryHandleWithdraw(player, item, amount);
                break;
            case Transaction.Send:
                TryHandleSend(player, recipient, item, amount);
                break;

        }
    }

    public static void HandleList(Player player)
    {
        var sb = new StringBuilder("\nBanked items:");

        foreach (var item in Settings.Items)
        {
            //Skip missing?
            var banked = player.GetBanked(item.Prop);
            var held = player.GetNumInventoryItemsOfWCID(item.Id);

            if (Settings.SkipMissingBankedItems && banked <= 0 && held <= 0)
                continue;

            sb.Append($"\n{banked,-25:0.00}{item.Name} banked, {held:0} held");
            //sb.Append($"\n{item.Name} (WCID={item.Id}):\n  {player.GetBanked(item.Prop):0.00} banked, {player.GetNumInventoryItemsOfWCID(item.Id):0.00} held");
        }

        player.SendMessage($"{sb}");
    }

    /// <summary>
    /// Attempts to withdraw an amount of a bank item 
    /// </summary>
    public static bool TryHandleWithdraw(Player player, BankItem item, int amount)
    {
        var banked = player.GetBanked(item.Prop);

        if (banked < amount)
        {
            player.SendMessage($"Unable to withdraw {amount}.  You have {banked} {item.Name}");
            return false;
        }

        //See if you can create items using the /ci approach
        if (player.TryCreateItems($"{item.Id} {amount}"))
        {
            player.IncBanked(item.Prop, -amount);
            player.SendMessage($"Withdrew {amount} {item.Name}. {player.GetBanked(item.Prop)} banked, {player.GetNumInventoryItemsOfWCID(item.Id)} held");
            return true;
        }
        else
        {
            player.SendMessage($"Failed to create {amount} {item.Name} in inventory.");
            return false;
        }
    }

    /// <summary>
    /// Attempts to deposit a bank item from inventory
    /// </summary>
    public static bool TryHandleDeposit(Player player, BankItem item, int amount)
    {
        if (player.TryTakeItems(item.Id, amount))
        {
            player.IncBanked(item.Prop, amount);
            player.SendMessage($"Deposited {amount:N0} {item.Name}. {player.GetBanked(item.Prop)} banked, {player.GetNumInventoryItemsOfWCID(item.Id):N0} held");
            return true;
        }

        player.SendMessage($"Unable to deposit {amount:N0}.  You have {player.GetNumInventoryItemsOfWCID(item.Id):N0} {item.Name}");
        return false;
    }

    /// <summary>
    /// Attempts to send a recipient some amount of a banked item
    /// </summary>
    public static bool TryHandleSend(Player player, string recipient, BankItem item, int amount)
    {
        var banked = player.GetBanked(item.Prop);
        if (banked < amount)
        {
            player.SendMessage($"Unable to withdraw {amount}.  You have {banked} {item.Name}");
            return false;
        }

        var alts = PlayerManager.GetAllOffline().Where(x => player.Account.AccountId == x.Account.AccountId);
        if (alts is null)
        {
            player.SendMessage($"No other players found on this account.");
            return false;
        }

        var r = alts.Where(x => x.Name.Contains(recipient, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        if (r is null)
        {
            player.SendMessage($"Recipient was not found: \n{string.Join("\n", alts.Select(x => $"{x.Name} - {x.Level}"))}");
            return false;
        }

        player.IncBanked(item.Prop, -amount);
        r.IncBanked(item.Prop, amount);

        player.SendMessage($"Sent {amount:N0} {item.Name}(s) from {player.Name} to {r.Name}.\n{banked - amount} remaining.");
        return true;
    }
}
