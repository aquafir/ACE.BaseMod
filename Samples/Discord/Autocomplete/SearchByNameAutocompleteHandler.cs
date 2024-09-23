namespace Discord.Autocomplete;


/// <summary>
/// Autocomplete based on Player's last appraised object's property values
/// </summary>
public class SearchByNameAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        //Get the focused value?
        var o = autocompleteInteraction.Data.Options;
        if (o is null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "Missing options data?");

        var option = o.Where(x => x.Focused)?.FirstOrDefault();

        if (option is null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "No parameter found.  Contact Bot smith.");

        var name = option.Value.ToString();

        //Try to find player?
        Player player = PlayerManager.FindByName(o.Where(x => x.Name == "player").FirstOrDefault()?.Value.ToString()) as Player;

        if (player is null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "Unable to find player.");

        var targetID = player.CurrentAppraisalTarget ?? 0;
        var target = player.FindObject(targetID, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (target is null)
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "Unable to find selection.");

        if (target is not Container container)
            //return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "Selection not a container.");
            container = player;


        //Make a regex
        var regex = new Regex(option.Value?.ToString() ?? "", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        //Use target's properties
        IEnumerable<AutocompleteResult> results = container.Inventory
            .Select(x => (x.Key, x.Value.Name))
            .Take(25)   //API max of 25
            .Where(x => regex.IsMatch(x.Name))
            .Select(x => new AutocompleteResult(x.Name, x.Key.Full));   //TODO: figure out returning guid?

        return AutocompletionResult.FromSuccess(results);
    }
}

