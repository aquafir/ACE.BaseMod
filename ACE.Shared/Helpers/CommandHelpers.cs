namespace ACE.Shared.Helpers;
public static class CommandHelpers
{
    public static bool TryIssueACECommand(string commandLine, string playerName = null)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
            return false;

        Session session = null;

        //Try to get player session if name provided
        if (!string.IsNullOrWhiteSpace(playerName))
        {
            var player = PlayerManager.GetOnlinePlayer(playerName);
            if (player is null)
            {
                return false;
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
            return false;
        }
        //Try to invoke
        try
        {
            var handlerResult = CommandManager.GetCommandHandler(session, command, parameters, out var commandHandler);
            //if (handlerResult == CommandHandlerResponse.Ok) {
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
            //}
        }
        catch (Exception ex)
        {
            ModManager.Log($"Exception while getting command handler for: {commandLine}", ModManager.LogLevel.Error);
        }

        return true;
    }



    public static async Task<bool> TryIssueACECommandAsync(string commandLine, string playerName = null)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
            return false;

        Session session = null;

        //Try to get player session if name provided
        if (!string.IsNullOrWhiteSpace(playerName))
        {
            var player = PlayerManager.GetOnlinePlayer(playerName);
            if (player is null)
            {
                return false;
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
            return false;
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

        return true;
    }
}
