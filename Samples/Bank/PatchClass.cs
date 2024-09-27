
namespace Bank;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

        if (Settings.VendorsUseBank)
            ModC.Harmony.PatchCategory(nameof(Debit));

        if (Settings.DirectDeposit)
            ModC.Harmony.PatchCategory(nameof(DirectDeposit));
    }

    public override void Stop()
    {
        base.Stop();

        if (Settings.VendorsUseBank)
            ModC.Harmony.UnpatchCategory(nameof(Debit));

        if (Settings.DirectDeposit)
            ModC.Harmony.UnpatchCategory(nameof(DirectDeposit));
}

    static string Currencies => string.Join(", ", Settings.Currencies.Select(x => x.Name));
    static string Commands => string.Join(", ", Enum.GetNames<Transaction>());

    [CommandHandler("bank", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleBank(Session session, params string[] parameters)
    {
        var player = session.Player;

        //Try to parse a valid command
        if (!parameters.TryParseCommand(out var verb, out var name, out var amount, out var wildcardAmount))
        {
            player.SendMessage($"Usage: <command> [name|id [amount=1]]\nAvailable commands: {Commands}");
            return;
        }

        //Parse weenie / amount if relevant to command
        if (verb == Transaction.Give || verb == Transaction.Take)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                player.SendMessage($"Specify the name or WCID of the item to transact with.");
                return;
            }


            //Try to parse weenie
            var query = int.TryParse(name, out var wcid) ?
                Settings.Items.Where(x => x.Id == wcid) :
                Settings.Items.Where(x => x.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

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

            if (verb == Transaction.Take)
                HandleWithdraw(player, item, amount);
            else
                HandleDeposit(player, item, amount);

            return;
        }

        //Handle other commands
        switch (verb)
        {
            case Transaction.List:
                var sb = new StringBuilder("\nBanked items:");
                foreach (var item in Settings.Items)
                    sb.Append($"\n{item.Name} (WCID={item.Id}):\n  {player.GetBanked(item.Prop)} banked, {player.GetNumInventoryItemsOfWCID(item.Id)} held");

                player.SendMessage($"{sb}");
                break;
        }
    }

    [CommandHandler("lum", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleLum(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;

        //Try to parse a valid command
        if (parameters.Length == 0 || !Enum.TryParse<Transaction>(parameters[0], true, out var verb))
        {
            player.SendMessage($"Usage: <command>: {Commands}");
            return;
        }


        switch (verb)
        {
            case Transaction.List:
                player.SendMessage($"You have {player.GetBanked(Settings.LuminanceProperty):N0} luminance.");
                break;
            case Transaction.Give:
                var available = player.AvailableLuminance ?? 0;

                if (player.SpendLuminance(available))
                {
                    player.IncBanked(Settings.LuminanceProperty, (int)available);
                    player.SendMessage($"Stored {available} luminance.  You now have {player.GetBanked(Settings.LuminanceProperty):N0}.");
                    return;
                }
                break;
            case Transaction.Take:
                var stored = player.GetBanked(Settings.LuminanceProperty);
                var max = player.MaximumLuminance ?? 0;
                var missing = max - (player.AvailableLuminance ?? 0);
                var withdraw = (int)Math.Min(missing, stored);

                player.GrantLuminance(withdraw, XpType.Admin, ShareType.None);
                player.IncBanked(Settings.LuminanceProperty, -withdraw);
                player.SendMessage($"You've withdrawn {withdraw} luminance.  You now have {player.GetBanked(Settings.LuminanceProperty):N0}.");
                break;
        }
    }

    [CommandHandler("cash", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleCash(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;


        if (!parameters.TryParseCommand(out var verb, out var name, out var amount, out var wildcardAmount))
        {
            player.SendMessage($"Usage: <command> [name|id [amount=1]]\nAvailable commands: {Commands}");
            return;
        }

        switch (verb)
        {
            case Transaction.List:
                player.SendMessage($"You have {player.GetBanked(Settings.CashProperty):N0}.\nCurrencies: {Currencies}");
                return;
            //Deposit everything
            case Transaction.Give:
                //Get coins and tradenotes
                var cashItems = player.AllItems().Where(x => x.WeenieClassId == Player.coinStackWcid || x.WeenieClassName.StartsWith("tradenote"));
                long total = 0;
                foreach (var item in cashItems)
                    total += item.Value ?? 0;
                var itemCount = cashItems.Count();

                foreach (var item in cashItems)
                {
                    //Remove and if it fails don't count the value of the item
                    if (!player.TryRemoveFromInventoryWithNetworking(item.Guid, out var consumed, Player.RemoveFromInventoryAction.ConsumeItem))
                    {
                        //Log?
                        total -= consumed.Value ?? 0;
                        itemCount--;
                    }
                }

                player.IncCash(total);
                player.SendMessage($"Deposited {itemCount} currency items for {total:N0}.  You have {player.GetBanked(Settings.CashProperty):N0}");
                return;

            case Transaction.Take:
                if (string.IsNullOrWhiteSpace(name))
                {
                    player.SendMessage($"Specify currency.");
                    return;
                }

                //Parse currency
                var currency = Settings.Currencies.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (currency is null || name == "")
                {
                    player.SendMessage($"Unable to find currency: {name}");
                    return;
                }

                //Withdraw amount
                long cost = (long)amount * currency.Value;

                //Check for overflow?

                long stored = player.GetBanked(Settings.CashProperty);
                if (stored < cost)
                {
                    //Todo: decide on withdrawing cap?
                    //if(wildcardAmount || Settings.ExcessSetToMax)

                    player.SendMessage($"Insufficient funds: {cost} < {stored}");
                    return;
                }

                if (player.TryCreateItems($"{currency.Id} {amount}"))
                {
                    player.IncCash(-cost);
                    player.SendMessage($"Withdrew {amount} {currency.Name} for {cost:N0}.  You have {player.GetBanked(Settings.CashProperty):N0} remaining.");
                }
                else
                    player.SendMessage($"Failed to withdraw {amount} {currency.Name} for {cost:N0}.  You have {player.GetBanked(Settings.CashProperty):N0} remaining.");

                //player.UpdateCoinValue();
                return;
        }
    }


    //Take items from the vault
    public static void HandleWithdraw(Player player, BankItem item, int amount)
    {
        var banked = player.GetBanked(item.Prop);

        if (banked < amount)
        {
            player.SendMessage($"Unable to withdraw {amount}.  You have {banked} {item.Name}");
            return;
        }

        //See if you can create items using the /ci approach
        if (player.TryCreateItems($"{item.Id} {amount}"))
        {
            player.IncBanked(item.Prop, -amount);
            player.SendMessage($"Withdrew {amount} {item.Name}. {player.GetBanked(item.Prop)} banked, {player.GetNumInventoryItemsOfWCID(item.Id)} held");
        }
    }

    public static void HandleDeposit(Player player, BankItem item, int amount)
    {
        if (player.TryTakeItems(item.Id, amount))
        {
            player.IncBanked(item.Prop, amount);
            player.SendMessage($"Deposited {amount:N0} {item.Name}. {player.GetBanked(item.Prop)} banked, {player.GetNumInventoryItemsOfWCID(item.Id):N0} held");
            return;
        }

        player.SendMessage($"Unable to deposit {amount:N0}.  You have {player.GetNumInventoryItemsOfWCID(item.Id):N0} {item.Name}");
    }
}

public static class BankExtensions
{
    public static long GetCash(this Player player) => player.GetBanked(PatchClass.Settings.CashProperty);
    public static void IncCash(this Player player, long amount)
    {
        player.IncBanked(PatchClass.Settings.CashProperty, amount);
        player.UpdateCoinValue();
    }

    public static long GetBanked(this Player player, int prop) =>
        player.GetProperty((PropertyInt64)prop) ?? 0;
    public static void IncBanked(this Player player, int prop, long amount) =>
        player.SetProperty((PropertyInt64)prop, player.GetBanked(prop) + amount);




    //Parsing
    static readonly string[] USAGES = new string[] {
        $@"(?<verb>{Transaction.List})$",
        //First check amount first cause I suck with regex
        $@"(?<verb>{Transaction.Give}|{Transaction.Take}) (?<name>.+)\s+(?<amount>(\*|\d+))$",
        $@"(?<verb>{Transaction.Give}|{Transaction.Take}) (?<name>.+)$",
        // /cash doesn't have named item
        $@"(?<verb>{Transaction.Give})$",
    };
    //Join usages in a regex pattern
    static string Pattern => string.Join("|", USAGES.Select(x => $"({x})"));
    static Regex CommandRegex = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool TryParseCommand(this string[] parameters, out Transaction verb, out string name, out int amount, out bool wildcardAmount)
    {
        //Set defaults
        amount = 1;
        verb = 0;
        name = null;
        wildcardAmount = false;

        //Debugger.Break();
        //Check for valid command
        var match = CommandRegex.Match(string.Join(" ", parameters));
        if (!match.Success)
            return false;

        //Parse verb
        if (!Enum.TryParse(match.Groups["verb"].Value, true, out verb))
            return false;

        //Set name
        name = match.Groups["name"].Value;

        //Parse amount if available
        if (int.TryParse(match.Groups["amount"].Value, out var parsedAmount))
            amount = parsedAmount;
        else if (match.Groups["amount"].Value == "*")
        {
            amount = int.MaxValue;
            wildcardAmount = true;
        }

        return true;
    }

    //Support for spaces in names
    public static string ParseName(this string[] param, int skip = 1, int atEnd = 0) => param.Length - skip - atEnd > 0 ?
        string.Join(" ", param.Skip(skip).Take(param.Length - atEnd - skip)) : "";

    //Parse quantity from last parameter supporting wildcards
    public static bool TryParseAmount(this string[] param, out int amount, int max = int.MaxValue)
    {
        var last = param.LastOrDefault() ?? "";

        //Default to 1
        amount = 1;

        bool success = true;

        //Check for wildcards/other handling
        if (last == "*")
            amount = int.MaxValue;
        else if (int.TryParse(last, out var parsedAmount))
            amount = parsedAmount;
        //Amount was not parsed
        else
            success = false;

        //Wildcards will always use the max value, parsed ints will use the setting
        if (PatchClass.Settings.ExcessSetToMax || last == "*")
            amount = Math.Min(max, amount);

        return success;
    }
}

public enum Transaction
{
    List,
    Give,
    Take,
    //Send,
}
