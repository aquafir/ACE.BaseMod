namespace DiscordPlus;

using ACE.Server.Managers;
using ACE.Server.Physics.Command;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;

public class CommandModule : ModuleBase<SocketCommandContext>
{
    //https://discordnet.dev/guides/text_commands/intro.html
    [SlashCommand("runas", "Run command as player")]
    [Discord.Commands.Summary("Issue chat command using approved list of Discord IDs as an online user?")]
    public async Task RunAsPlayer(
        [Discord.Commands.Summary("Player to run command")]
        string player,
        [Discord.Commands.Summary("Command to run")]
        string command)
    {
        if (PatchClass.Settings.DevIds.Contains(Context.User.Id))
        {
            IssueACECommand(command, player);
        }
    }

    [Command("run")]
    [Discord.Commands.Summary("Issue chat command using approved list of Discord IDs?")]
    public async Task RunAsDev(
        [Discord.Commands.Summary("Command to run")]
        string command)
    {
        if (PatchClass.Settings.DevIds.Contains(Context.User.Id))
        {
            IssueACECommand(command);
        }
    }

    private async void IssueACECommand(string commandLine, string playerName = null)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
            return;

        Session session = null;

        //Try to get player session if name provided
        if (!String.IsNullOrWhiteSpace(playerName))
        {

            var player = PlayerManager.GetOnlinePlayer(playerName);
            if (player is null)
            {
                await Context.Channel.SendMessageAsync($"Failed to find player: {playerName}");
                return;
            }
            session = player.Session;
        }

        //Parse command
        string command = null;
        string[] parameters = null;
        try
        {
            CommandManager.ParseCommand(commandLine, out command, out parameters);
        }
        catch (Exception ex)
        {
            ModManager.Log($"Exception while parsing command: {commandLine}", ModManager.LogLevel.Error);
            return;
        }
        //Try to invoke
        try
        {
            if (CommandManager.GetCommandHandler(session, command, parameters, out var commandHandler) == CommandHandlerResponse.Ok)
            {
                try
                {
                    if (commandHandler.Attribute.IncludeRaw)
                    {
                        parameters = CommandManager.StuffRawIntoParameters(commandLine, command, parameters);
                    }
                    // Add command to world manager's main thread...
                    ((CommandHandler)commandHandler.Handler).Invoke(session, parameters);
                }
                catch (Exception ex)
                {
                    ModManager.Log($"Exception while invoking command handler for: {commandLine}", ModManager.LogLevel.Error);
                }
            }
        }
        catch (Exception ex)
        {
            ModManager.Log($"Exception while getting command handler for: {commandLine}", ModManager.LogLevel.Error);
        }
    }
}
