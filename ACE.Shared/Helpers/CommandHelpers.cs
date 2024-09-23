//namespace ACE.Shared.Helpers;
//public static class CommandHelpers
//{
//    static readonly Dictionary<Type, Regex> commandVerbs = new();

//    public static string JoinString(this string[] parameters) => string.Join(' ', parameters);
//    /// <summary>
//    /// Parses a command 
//    /// </summary>
//    public static bool TryParseCommand<TEnum>(this string[] command, out TEnum result, out string parameters) where TEnum : struct, Enum
//    {
//        parameters = null;
//        var type = typeof(TEnum);

//        if (!commandVerbs.TryGetValue(type, out var reg))
//        {
//            string verbs = string.Join("|", Enum.GetNames(type));
//            string pattern = $"(?<verb>{verbs})(?<params>.*)";
//            reg = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

//            commandVerbs.Add(type, reg);
//        }

//    }

//    //=> TryParseCommand<TEnum>(command.JoinString(), out result, out parameters);

//    public static bool TryParseCommand<TEnum>(string command, out TEnum result, out string parameters) where TEnum : struct, Enum
//    {
//        var type = typeof(TEnum);

//        if (!commandVerbs.TryGetValue(type, out var reg))
//        {
//            string verbs = string.Join("|", Enum.GetNames(type));
//            string pattern = $"(?<verb>{verbs})(?<params>.*)";
//            reg = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

//            commandVerbs.Add(type, reg);
//        }

//        // Iterate over all enum values and try to find a match at the start of the string
//        foreach (var enumValue in Enum.GetValues(typeof(TEnum)))
//        {
//            string enumName = enumValue.ToString();
//            if (joinedString.StartsWith(enumName, StringComparison.OrdinalIgnoreCase))
//            {
//                // Try to parse the matched substring as the enum type
//                return Enum.TryParse(enumName, out result);
//            }
//        }

//        // If no match is found, set the result to default and return false
//        result = default;
//        return false;
//    }

//    public static bool TryIssueACECommand(string commandLine, string playerName = null)
//    {
//        if (string.IsNullOrWhiteSpace(commandLine))
//            return false;

//        Session session = null;

//        //Try to get player session if name provided
//        if (!string.IsNullOrWhiteSpace(playerName))
//        {
//            var player = PlayerManager.GetOnlinePlayer(playerName);
//            if (player is null)
//            {
//                return false;
//            }
//            session = player.Session;
//        }

//        //Parse command
//        string command = null;
//        string[] parameters = null;
//        try
//        {
//            CommandManager.ParseCommand(commandLine, out command, out parameters);
//        }
//        catch (Exception ex)
//        {
//            ModManager.Log($"Exception while parsing command: {commandLine}", ModManager.LogLevel.Error);
//            return false;
//        }
//        //Try to invoke
//        try
//        {
//            var handlerResult = CommandManager.GetCommandHandler(session, command, parameters, out var commandHandler);
//            //if (handlerResult == CommandHandlerResponse.Ok) {
//            try
//            {
//                if (commandHandler.Attribute.IncludeRaw)
//                {
//                    parameters = CommandManager.StuffRawIntoParameters(commandLine, command, parameters);
//                }
//                // Add command to world manager's main thread...
//                ((CommandHandler)commandHandler.Handler).Invoke(session, parameters);
//            }
//            catch (Exception ex)
//            {
//                ModManager.Log($"Exception while invoking command handler for: {commandLine}", ModManager.LogLevel.Error);
//            }
//            //}
//        }
//        catch (Exception ex)
//        {
//            ModManager.Log($"Exception while getting command handler for: {commandLine}", ModManager.LogLevel.Error);
//        }

//        return true;
//    }

//    public static async Task<bool> TryIssueACECommandAsync(string commandLine, string playerName = null)
//    {
//        if (string.IsNullOrWhiteSpace(commandLine))
//            return false;

//        Session session = null;

//        //Try to get player session if name provided
//        if (!string.IsNullOrWhiteSpace(playerName))
//        {
//            var player = PlayerManager.GetOnlinePlayer(playerName);
//            if (player is null)
//            {
//                return false;
//            }
//            session = player.Session;
//        }

//        //Parse command
//        string command = null;
//        string[] parameters = null;
//        try
//        {
//            CommandManager.ParseCommand(commandLine, out command, out parameters);
//        }
//        catch (Exception ex)
//        {
//            ModManager.Log($"Exception while parsing command: {commandLine}", ModManager.LogLevel.Error);
//            return false;
//        }
//        //Try to invoke
//        try
//        {
//            if (CommandManager.GetCommandHandler(session, command, parameters, out var commandHandler) == CommandHandlerResponse.Ok)
//            {
//                try
//                {
//                    if (commandHandler.Attribute.IncludeRaw)
//                    {
//                        parameters = CommandManager.StuffRawIntoParameters(commandLine, command, parameters);
//                    }
//                    // Add command to world manager's main thread...
//                    ((CommandHandler)commandHandler.Handler).Invoke(session, parameters);
//                }
//                catch (Exception ex)
//                {
//                    ModManager.Log($"Exception while invoking command handler for: {commandLine}", ModManager.LogLevel.Error);
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            ModManager.Log($"Exception while getting command handler for: {commandLine}", ModManager.LogLevel.Error);
//        }

//        return true;
//    }
//}
