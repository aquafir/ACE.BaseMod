namespace Tower;

public static class BankExtensions
{
    static BankSettings Settings => PatchClass.Settings.Bank;

    public static double GetBanked(this Player player, int prop) =>
        player.GetProperty((PropertyFloat)prop) ?? 0;
    public static void IncBanked(this Player player, int prop, double amount) =>
        player.SetProperty((PropertyFloat)prop, player.GetBanked(prop) + amount);

    public static double GetBanked(this OfflinePlayer player, int prop) =>
        player.GetProperty((PropertyFloat)prop) ?? 0;
    public static void IncBanked(this OfflinePlayer player, int prop, double amount) =>
        player.SetProperty((PropertyFloat)prop, player.GetBanked(prop) + amount);

    private static Dictionary<uint, BankItem> bankLookup = null;
    /// <summary>
    /// Called on plugin startup to populate a lookup for bankable items
    /// </summary>
    public static void Init()
    {
        bankLookup = Settings.Items.ToDictionary(x => x.Id, x => x);
    }
    /// <summary>
    /// Try to find the corresponding BankItem for a WorldObject
    /// </summary>
    public static bool TryGetBankedItem(this WorldObject wo, out BankItem item) =>
        bankLookup.TryGetValue(wo.WeenieClassId, out item);

    //Parsing
    static readonly string[] USAGES = new string[] {
        $@"(?<verb>{Transaction.List})$",
        //First check amount first cause I suck with regex
        $@"(?<verb>{Transaction.Give}|{Transaction.Take}) (?<name>.+)\s+(?<amount>(\*|\d+))$",
        $@"(?<verb>{Transaction.Give}|{Transaction.Take}) (?<name>.+)$",
        // /cash doesn't have named item
        $@"(?<verb>{Transaction.Give})$",
        $@"(?<verb>{Transaction.Send}) (?<recipient>.+) (?<name>.+)\s+(?<amount>(\*|\d+))$",
        $@"(?<verb>{Transaction.Send}) (?<recipient>.+) (?<name>.+)$",
    };
    //Join usages in a regex pattern
    static string Pattern => string.Join("|", USAGES.Select(x => $"({x})"));
    static Regex CommandRegex = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool TryParseCommand(this string[] parameters, out Transaction verb, out string name, out int amount, out bool wildcardAmount, out string recipient)
    {
        //Set defaults
        amount = 1;
        verb = 0;
        name = null;
        recipient = null;
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

        //Set recipient if available
        recipient = match.Groups["recipient"].Value;

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
        if (Settings.ExcessSetToMax || last == "*")
            amount = Math.Min(max, amount);

        return success;
    }
}


public enum Transaction
{
    List,
    Give,
    Take,
    Send,
}
