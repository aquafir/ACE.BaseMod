namespace Discord.Autocomplete;

/// <summary>
/// Autocomplete for full property
/// </summary>
public class PropertyStringAutocompleteHandler : AutocompleteHandler
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

        // max - 25 suggestions at a time (API limit)
        IEnumerable<AutocompleteResult> results = Enum.GetNames<PropertyString>()
            .Where(x => x.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(x => new AutocompleteResult(x, x));

        return AutocompletionResult.FromSuccess(results);
    }
}

/// <summary>
/// Autocomplete based on Player's property values
/// </summary>
public class PlayerPropertyStringAutocompleteHandler : AutocompleteHandler
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

        //Use target's properties
        IEnumerable<AutocompleteResult> results = player.GetAllPropertyString().Keys
            .Take(25)   //API max of 25
            .Select(x => x.ToString())
            .Where(x => x.Contains(option.Value?.ToString(), StringComparison.OrdinalIgnoreCase))
            .Select(x => new AutocompleteResult(x, x));

        return AutocompletionResult.FromSuccess(results);
    }
}

/// <summary>
/// Autocomplete based on Player's last appraised object's property values
/// </summary>
public class SelectedPropertyStringAutocompleteHandler : AutocompleteHandler
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

        //Use target's properties
        IEnumerable<AutocompleteResult> results = target.GetAllPropertyString().Keys
            .Select(x => x.ToString())
            .Take(25)   //API max of 25
            .Where(x => x.Contains(option.Value?.ToString(), StringComparison.OrdinalIgnoreCase))
            .Select(x => new AutocompleteResult(x, x));

        return AutocompletionResult.FromSuccess(results);
    }
}

