using AltV.Net;
using AltV.Net.Elements.Entities;
using System.Reflection;
using System.Text.Json;

namespace Altv.VChat.Server;

public static class VChat
{
    private static CommandSuggestionRoot root = new CommandSuggestionRoot();
    private static Action<string, string> registerCmd;

    public const string CommandPrefix = "CMD_";
    public const string ModuleName = "altv-vchat";

    private static Func<Type, object> ScriptActivator { get; set; }
    public static void Init(Func<Type, object> scriptActivator = null, params Assembly[] assemblies)
    {
        ScriptActivator = scriptActivator;

        if (ScriptActivator == null)
        {
            ScriptActivator = Activate;
        }

        Alt.Import(ModuleName, "registerCallbackCmd", out registerCmd);

        foreach (var assembly in assemblies)
        {
            var scriptTypes = assembly.GetTypes().Where(x => typeof(IScript).IsAssignableFrom(x));

            foreach (var scriptType in scriptTypes)
            {
                FindMethods(scriptType);
            }
        }

        var json = JsonSerializer.Serialize(root, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });

        File.WriteAllText($"resources/{ModuleName}/commands.json", json);
    }

    private static void FindMethods(Type? scriptType)
    {
        foreach (var method in scriptType.GetMethods())
        {
            TryRegisterCommand(method);
        }
    }

    private static void TryRegisterCommand(MethodInfo method)
    {
        var commandAttribute = method.GetCustomAttribute<CommandDefAttribute>();

        if (commandAttribute != null)
        {
            try
            {
                var callback = CommandPrefix + commandAttribute.CommandName;

                registerCmd(commandAttribute.CommandName, callback);

                var methodParameters = method.GetParameters();

                //Alt.OnServer<IPlayer, string[]>(callback, (player, arguments) =>
                //{
                //    var obj = ScriptActivator.Invoke(method.DeclaringType);

                //    List<object> sendingParameters = new List<object>();

                //    sendingParameters.Add(player);

                //    // arguments[0] -> methodParameters[1]

                //    for (int i = 0; i < arguments.Length; i++)
                //    {
                //        var typeOfArgument = methodParameters[i + 1].ParameterType;
                //        sendingParameters.Add(Convert.ChangeType(arguments[i], typeOfArgument));
                //    }

                //    for (int i = 0; i < methodParameters.Length - sendingParameters.Count; i++)
                //    {
                //        sendingParameters.Add(Type.Missing);
                //    }

                //    method.Invoke(obj, sendingParameters.ToArray());

                //});

                var commandSuggestion = new CommandSuggestion
                {
                    Name = commandAttribute.CommandName,
                    Description = commandAttribute.Description,
                    Parameters = methodParameters.Skip(1).Select((x, i) => new CommandSuggestionParameter
                    {
                        Name = x.Name,
                        //Description = commandAttribute.ArgumentDescriptions.Length > i - 1 ? commandAttribute.ArgumentDescriptions[i] : string.Empty
                    }).ToArray()
                };

                root.CommandSuggestions.Add(commandSuggestion);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private static object Activate(Type type)
    {
        return Activator.CreateInstance(type);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class CommandDefAttribute : Attribute
{
    public CommandDefAttribute(string commandName, string description = null, params string[] argumentDescriptions)
    {
        this.CommandName = commandName;
        this.Description = description;
        ArgumentDescriptions = argumentDescriptions;
    }

    public string CommandName { get; }

    public string Description { get; }

    public string[] ArgumentDescriptions { get; }
}