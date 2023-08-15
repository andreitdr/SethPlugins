using Discord.WebSocket;
using PluginManager.Interfaces;
using PluginManager.Others;
using Python.Runtime;

namespace PythonCompatibilityLayer;

public class Entry : DBEvent
{
    public string Name => "PythonCompatibilityLayer";
    public string Description => "A compatibility layer for Python plugins.";
    public void Start(DiscordSocketClient client)
    {
        Console.WriteLine("PythonCompatibilityLayer: Starting Python runtime.");
        if(!Variables.Commands.ContainsKey("python_dll"))
            throw new Exception("PythonCompatibilityLayer: Python DLL not found. Please add it to the config file.\nUse the command \"registercmd python_dll <path>\" to register it.");
        Runtime.PythonDLL = Variables.Commands["python_dll"];
        
        client.MessageReceived += async message =>
        {
            if (message.Author.IsBot) return;
            DBCommandExecutingArguments args = new(message as SocketUserMessage, client);
            
            Console.WriteLine($"PythonCompatibilityLayer: Received command {args.commandUsed}");

            if (Variables.Commands.ContainsKey(args.commandUsed))
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    Console.WriteLine($"PythonCompatibilityLayer: Command found. {args.commandUsed}");
                    Console.WriteLine($"Imporing : {Variables.Commands[args.commandUsed]}");
                    dynamic scr = Py.Import(Variables.Commands[args.commandUsed]);
                    Console.WriteLine($"PythonCompatibilityLayer: Script imported. {Variables.Commands[args.commandUsed]}");
                    dynamic result = scr.hello_world();
                    Console.WriteLine(result);
                    await message.Channel.SendMessageAsync(result.ToString());
                }
                
                PythonEngine.Shutdown();

                
            }
        };
        Console.WriteLine("PythonCompatibilityLayer: Python runtime started.");
        Console.WriteLine("If you see any errors above, please make sure you have Python installed and added to your PATH.");
    }
}