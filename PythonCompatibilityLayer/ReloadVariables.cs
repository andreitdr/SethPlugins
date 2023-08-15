using PluginManager.Interfaces;
using PluginManager.Others;

namespace PythonCompatibilityLayer;

public class ReloadVariables : ICommandAction
{
    public string ActionName => "py_reloadvariables";
    public string? Description => "Reloads the variables from the variables.json file.";
    public string? Usage => "py_reloadvariables";
    public InternalActionRunType RunType => InternalActionRunType.ON_CALL;
    public Task Execute(string[]? args)
    {
        Variables.Commands = new PythonCommands("./Data/Resources/pythonCommands.json");
        return Task.CompletedTask;
    }
}