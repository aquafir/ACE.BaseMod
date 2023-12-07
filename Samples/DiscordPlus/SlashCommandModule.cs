using Discord.Commands;
using Discord.Interactions;

namespace DiscordPlus;
public class SlashCommandModule : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [SlashCommand("ping", "Ping-pong!")]
    public async Task PingAsync()
    {
        await ReplyAsync("Pong!");
    }
}

