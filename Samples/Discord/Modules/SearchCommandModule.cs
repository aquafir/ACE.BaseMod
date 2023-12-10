namespace Discord.Modules;

public class SearchCommandModule : InteractionModuleBase<SocketInteractionContext>
{ 
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
                sb.AppendLine($"\n## Bools:\n{String.Join("\n", target.GetAllPropertyBools().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n## Floats:\n{String.Join("\n", target.GetAllPropertyFloat().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n## Ints:\n{String.Join("\n", target.GetAllPropertyInt().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n## Int64s:\n{String.Join("\n", target.GetAllPropertyInt64().Select(x => $"{x.Key} = {x.Value}"))}");
                sb.AppendLine($"\n## Strings:\n{String.Join("\n", target.GetAllPropertyString().Select(x => $"{x.Key} = {x.Value}"))}");

                await RespondAsync($"{sb.ToString()}");
                //await RespondAsync($"Guid of item: {query}");
            }
        }
    }
}
