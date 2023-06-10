namespace Altv.VChat.Server;

public class CommandSuggestionRoot
{
    public List<CommandSuggestion> CommandSuggestions { get; } = new();
}

public class CommandSuggestion
{
    public string Name { get; set; }
    public string Description { get; set; }
    public CommandSuggestionParameter[] Parameters { get; set; }
}

public class CommandSuggestionParameter
{
    public string Name { get; set; }
    public string Description { get; set; }
}
