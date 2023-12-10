using Newtonsoft.Json.Linq;
using System;

namespace Discord.Modules;

public class PropertyCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
    [Group("set", "Set property values")]
    public class ModGroup : InteractionModuleBase<SocketInteractionContext>
    {
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

            var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyBool>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    target.EnqueueBroadcastUpdateObject();
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }

        [SlashCommand("did", "Modify last appraised")]
        public async Task ModifyDataId(
    [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
    [Summary("Prop"), Autocomplete(typeof(SelectedPropertyDataIdAutocompleteHandler))] string prop,
    uint value
    )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyDataId>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    target.EnqueueBroadcastUpdateObject();
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }

        [SlashCommand("iid", "Modify last appraised")]
        public async Task ModifyInstanceId(
    [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
    [Summary("Prop"), Autocomplete(typeof(SelectedPropertyInstanceIdAutocompleteHandler))] string prop,
    uint value
    )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyInstanceId>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    target.EnqueueBroadcastUpdateObject();
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }

        [SlashCommand("float", "Modify last appraised")]
        public async Task ModifyFloat(
[Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
[Summary("Prop"), Autocomplete(typeof(SelectedPropertyFloatAutocompleteHandler))] string prop,
double value
)
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyFloat>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    target.EnqueueBroadcastUpdateObject();
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }


        [SlashCommand("int", "Modify last appraised")]
        public async Task ModifyInt(
    [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
    [Summary("Prop"), Autocomplete(typeof(SelectedPropertyIntAutocompleteHandler))] string prop,
    int value
    )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyInt>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    target.EnqueueBroadcastUpdateObject();
                    await RespondAsync($"{target.Name}'s {propEnum}: {old.ToString() ?? "null"}->{value}");
                }
            }
        }

        [SlashCommand("int64", "Modify last appraised")]
        public async Task ModifyInt64(
    [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
    [Summary("Prop"), Autocomplete(typeof(SelectedPropertyInt64AutocompleteHandler))] string prop,
    long value
    )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{player} missing");
            else
            {
                if (Enum.TryParse<PropertyInt64>(prop, out var propEnum))
                {
                    var old = target.GetProperty(propEnum);
                    target.SetProperty(propEnum, value);
                    target.EnqueueBroadcastUpdateObject();
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

        var target = p.FindObject(p.CurrentAppraisalTarget ?? 0, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (target is null)
            await RespondAsync($"{player} missing");
        else
        {
            if (Enum.TryParse<PropertyString>(prop, out var propEnum))
            {
                var old = target.GetProperty(propEnum);
                target.SetProperty(propEnum, value);
                target.EnqueueBroadcastUpdateObject();
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

    [Group("search", "Find items")]
    public class SearchGroup : InteractionModuleBase<SocketInteractionContext>
    {
        //Search player's last appraised for item?  Or just player themself?
        [SlashCommand("name", "Search by name regex")]
        public async Task ModifyBool(
            [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
            [Summary("Query"), Autocomplete(typeof(SearchByNameAutocompleteHandler))] uint query
            )
        {
            var p = PlayerManager.FindByName(player ?? "") as Player;
            if (p is null)
                await RespondAsync($"Could not find {player}");

            var target = p.FindObject(query, Player.SearchLocations.Everywhere, out _, out _, out _);
            if (target is null)
                await RespondAsync($"{query} missing");
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Name: {target.Name}");
                sb.AppendLine($"Guid: {query}");
                sb.AppendLine($"*Bools:*\n{String.Join("\n", target.GetAllPropertyBools().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n*Floats:*\n{String.Join("\n", target.GetAllPropertyFloat().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n*Ints:*\n{String.Join("\n", target.GetAllPropertyInt().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n*Int64s:*\n{String.Join("\n", target.GetAllPropertyInt64().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n*Strings:*\n{String.Join("\n", target.GetAllPropertyString().Select(x => $"{x.Key} = {x.Value}"))}");

                await RespondAsync($"{sb.ToString()}");
                //await RespondAsync($"Guid of item: {query}");
            }
        }
    }

}
