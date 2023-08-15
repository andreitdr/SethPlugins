using Discord.Commands;
using Discord.WebSocket;
using PluginManager;
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
        
        client.MessageReceived += async Message =>
        {
            if (Message.Author.IsBot)
                return;

            if (Message as SocketUserMessage == null)
                return;

            var message = Message as SocketUserMessage;

            if (message is null)
                return;

            var argPos = 0;

            if (!message.Content.StartsWith(Config.DiscordBot.botPrefix) && !message.HasMentionPrefix(client.CurrentUser, ref argPos))
                return;
            
            DBCommandExecutingArguments args = new(message as SocketUserMessage, client);
            if (Variables.Commands.TryGetValue(args.commandUsed, out var command))
            {
                PythonEngine.Initialize();
                
                using (Py.GIL())
                {
                    dynamic scr = Py.Import(command);
                    dynamic result = scr.Execute(args.arguments);
                    await message.Channel.SendMessageAsync(result.ToString());
                }
                
                PythonEngine.Shutdown();

                
            }
        };
        Console.WriteLine("PythonCompatibilityLayer: Python runtime started.");
        Console.WriteLine("If you see any errors above, please make sure you have Python installed and added to your PATH.");
    }
}