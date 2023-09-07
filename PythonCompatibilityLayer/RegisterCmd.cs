using PluginManager;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace PythonCompatibilityLayer;

public class RegisterCmd : ICommandAction
{
    public string ActionName => "registercmd";
    public string? Description => "Registers a command for the PythonCompatibilityLayer plugin.";
    public string? Usage => "registercmd <command> <script_path>";
    public InternalActionRunType RunType => InternalActionRunType.ON_CALL;
    public async Task Execute(string[]? args)
    {
        string command = args![0];
        string scriptPath = string.Join(' ', args, 1, args.Length - 1);

        if (!Variables.Commands.ContainsKey(command))
        {
            Variables.Commands.Add(command, scriptPath);
        }

        await Variables.Commands.SaveToFile();
    }
}