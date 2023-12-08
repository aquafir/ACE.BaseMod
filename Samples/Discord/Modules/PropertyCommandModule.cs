using GroupAttribute = Discord.Interactions.GroupAttribute;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;

namespace Discord.Modules;

public class PropertyCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
    [Group("set", "Set property values")]
    public class ModGroup : InteractionModuleBase<SocketInteractionContext>
    {
        //[SlashCommand("bool2", "Modify player")]
        //public async Task ModifyBool2(
        //   [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
        //   [Summary("PropType"), Autocomplete(typeof(PlayerPropertyBoolAutocompleteHandler))] string prop
        //   )
        //{
        //    var p = PlayerManager.FindByName(player ?? "") as Player;
        //    if (p is null)
        //        await RespondAsync($"Could not find {player}");

        //    if (Enum.TryParse<PropertyBool>(prop, out var propEnum))
        //    {
        //        var val = p.GetProperty(propEnum);
        //        await RespondAsync($"Got {propEnum} of {player}: {val}");
        //    }
        //}

        [SlashCommand("bool", "Modify last appraised")]
        public async Task ModifyBool(
            [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
            [Summary("Prop"), Autocomplete(typeof(SelectedPropertyBoolAutocompleteHandler))] string prop,
            bool value
            )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.RequestedAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyBool>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }

        [SlashCommand("string", "Modify last appraised")]
        public async Task ModifyString(
            [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
            [Summary("Prop"), Autocomplete(typeof(SelectedPropertyStringAutocompleteHandler))] string prop,
            string value
            )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.RequestedAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyString>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    p.UpdateProperty(target, propEnum, value, true);                    
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }

    }

    [Group("get", "View properties")]
    public class GetGroup : InteractionModuleBase<SocketInteractionContext>
    {
        // You can create command choices either by using the [Choice] attribute or by creating an enum. Every enum with 25 or less values will be registered as a multiple
        // choice option
        //[SlashCommand("choice_example", "Enums create choices")]
        //public async Task ChoiceExample(ExampleEnum input)
        //    => await RespondAsync(input.ToString());

        [SlashCommand("bool", "Modify last appraised")]
        public async Task ModifyBool(
            [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
            [Summary("PropType"), Autocomplete(typeof(PropertyStringAutocompleteHandler))] string prop
            )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
            {
                // await RespondAsync($"Could not find {player}");
            }
            else
            {
                // await RespondAsync($"Kicked {player}");
            }
            //Debugger.Break();
            await RespondAsync($"{player} {prop}");
        }
    }
}
