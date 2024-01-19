using ACE.Database;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Command.Handlers;
using ACE.Server.WorldObjects;
using ACE.Shared;
using ACE.Shared.Helpers;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Bank;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    const int RETRIES = 10;

    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);

    private JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private void LoadSettings()
    {
        if (!settingsInfo.Exists)
        {
            ModManager.Log($"Creating {settingsInfo}...");
            SaveSettings();
        }
        else
            ModManager.Log($"Loading settings from {settingsPath}...");

        if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
        {
            Mod.State = ModState.Error;
            return;
        }

        try
        {
            Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
        }
        catch (Exception)
        {
            ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
            return;
        }
    }
    #endregion

    #region Start/Shutdown
    public void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;
        LoadSettings();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    static string Currencies => String.Join(", ", Settings.Currencies.Select(x => x.Name));
    static string Commands => String.Join(", ", Enum.GetNames<Transaction>());

    [CommandHandler("bank", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleBank(Session session, params string[] parameters)
    {
        var player = session.Player;

        //Try to parse a valid command
        if (parameters.Length == 0 || !Enum.TryParse<Transaction>(parameters[0], true, out var command))
        {
            player.SendMessage($"Usage: <command> [name|id [amount=1|*]]\nAvailable commands: {Commands}");
            return;
        }

        //Parse weenie / amount if relevant to command
        if ((command == Transaction.Give || command == Transaction.Take))
        {
            if (parameters.Length < 1)
            {
                player.SendMessage($"Specify the name or WCID of the item to transact with.");
                return;
            }

            //Try to parse weenie
            var query = int.TryParse(parameters[1], out var wcid) ?
                Settings.Items.Where(x => x.Id == wcid) :
                Settings.Items.Where(x => x.Name.StartsWith(parameters[1], StringComparison.OrdinalIgnoreCase));

            var item = query.FirstOrDefault();
            if (item is null)
            {
                player.SendMessage($"Unable to find matching item: {parameters[1]}");
                return;
            }

            //Parse amount or default to 1
            var amount = 1;
            if (parameters.Length > 2)
            {
                //Try wildcard first
                if (parameters[2] == "*")
                    amount = command == Transaction.Give ? player.GetNumInventoryItemsOfWCID(item.Id) : (int)player.GetBanked(item.Prop);
                //Try to parse quantity
                else if (!int.TryParse(parameters[2], out amount))
                {
                    player.SendMessage($"Unable to parse amount from: {parameters[2]}");
                    return;
                }
            }

            //Take the cap if it's smaller
            if (Settings.ExcessSetToMax)
                amount = Math.Min(amount, command == Transaction.Give ? player.GetNumInventoryItemsOfWCID(item.Id) : (int)player.GetBanked(item.Prop));

            if (command == Transaction.Take)
                HandleWithdraw(player, item, amount);
            else
                HandleDeposit(player, item, amount);

            return;
        }

        //Handle other commands
        switch (command)
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
        if (parameters.Length == 0 || !Enum.TryParse<Transaction>(parameters[0], true, out var command))
        {
            player.SendMessage($"Usage: <command>: {Commands}");
            return;
        }


        switch (command)
        {
            case Transaction.List:
                player.SendMessage($"You have {player.GetBanked(Settings.LuminanceProperty):N} luminance.");
                break;
            case Transaction.Give:
                var available = player.AvailableLuminance ?? 0;

                if(player.SpendLuminance(available))
                {
                    player.IncBanked(Settings.LuminanceProperty, (int)available);
                    player.SendMessage($"Stored {available} luminance.  You now have {player.GetBanked(Settings.LuminanceProperty):N}.");
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
                player.SendMessage($"You've withdrawn {withdraw} luminance.  You now have {player.GetBanked(Settings.LuminanceProperty):N}.");
                break;
        }
    }

    [CommandHandler("cash", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleCash(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;

        //Try to parse a valid command
        if (parameters.Length == 0 || !Enum.TryParse<Transaction>(parameters[0], true, out var command))
        {
            player.SendMessage($"Usage: <command> [name|id [amount=1]]\nAvailable commands: {Commands}");
            return;
        }


        switch (command)
        {
            case Transaction.List:
                player.SendMessage($"You have {player.GetBanked(Settings.CashProperty):N}.\nCurrencies: {Currencies}");
                return;
            //Deposit everything
            case Transaction.Give:
                //Get coins and tradenotes
                var cashItems = player.Inventory.Where(x => x.Value.WeenieType == WeenieType.Coin || x.Value.WeenieClassName.StartsWith("tradenote"));

                var total = cashItems.Select(x => x.Value.Value)?.Sum() ?? 0;
                var itemCount = cashItems.Count();

                foreach (var item in cashItems)
                {
                    //Remove and if it fails don't count the value of the item
                    if (!player.TryRemoveFromInventoryWithNetworking(item.Key, out var consumed, Player.RemoveFromInventoryAction.ConsumeItem))
                    {
                        //Log?
                        total -= consumed.Value ?? 0;
                        itemCount--;
                    }
                }

                player.IncBanked(Settings.CashProperty, total);
                player.SendMessage($"Deposited {itemCount} currency items for {total:N}.  You have {player.GetBanked(Settings.CashProperty):N}");
                return;

            case Transaction.Take:
                if (parameters.Length < 2)
                {
                    player.SendMessage($"Specify currency.");
                    return;
                }

                //Parse currency
                var currency = Settings.Currencies.Where(x => x.Name.Equals(parameters[1], StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (currency is null)
                {
                    player.SendMessage($"Unable to find currency: {parameters[1]}");
                    return;
                }

                //Parse amount
                var amount = 1;
                if (parameters.Length == 3 && !int.TryParse(parameters[2], out amount))
                {
                    player.SendMessage($"Unable to parse amount: {parameters[2]}");
                    return;
                }

                //Withdraw amount
                int cost = amount * currency.Value;
                long stored = player.GetBanked(Settings.CashProperty);
                if (stored < cost)
                {
                    player.SendMessage($"Insufficient funds: {cost} < {stored}");
                    return;
                }

                if (player.TryCreateItems($"{currency.Id} {amount}"))
                {
                    player.IncBanked(Settings.CashProperty, -cost);
                    player.SendMessage($"Withdrew {amount} {currency.Name} for {cost}.  You have {player.GetBanked(Settings.CashProperty):N} remaining.");
                }
                else
                    player.SendMessage($"Failed to withdraw {amount} {currency.Name} for {cost}.  You have {player.GetBanked(Settings.CashProperty):N} remaining.");
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
            player.SendMessage($"Deposited {amount} {item.Name}. {player.GetBanked(item.Prop)} banked, {player.GetNumInventoryItemsOfWCID(item.Id)} held");
            return;
        }

        player.SendMessage($"Unable to deposit {amount}.  You have {player.GetNumInventoryItemsOfWCID(item.Id)} {item.Name}");
    }

}

public static class BankExtensions
{
    public static long GetBanked(this Player player, int prop) =>
        player.GetProperty((PropertyInt64)prop) ?? 0;
    public static void IncBanked(this Player player, int prop, int amount) =>
            player.SetProperty((PropertyInt64)prop, player.GetBanked(prop) + amount);
}

public enum Transaction
{
    List,
    Give,
    Take,
    //Send,
}
