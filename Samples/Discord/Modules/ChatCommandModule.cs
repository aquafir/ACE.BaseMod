using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Modules;
public class ChatCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }
    private InteractionHandler _handler;
    public ChatCommandModule(InteractionHandler handler) => _handler = handler;

    // you need to add `Autocomplete` attribute before parameter to add autocompletion to it
    [SlashCommand("t", "Message player")]
    public async Task TellCommand(
        [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
        string message
        )

    {
        var p = PlayerManager.FindByName(player ?? "") as Player;
        if (p is null)
            await RespondAsync($"Could not find {player}");
        else
        {
            p.SendMessage(message);
            await RespondAsync();
            DeleteOriginalResponseAsync();
        }
    }
}
