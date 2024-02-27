using PluginManager;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace LevelingSystem;

public class ReloadAction: ICommandAction
{
    public string ActionName => "LevelingSystemReload";
    public string? Description => "Reloads the Leveling System config file";
    public string? Usage => "LevelingSystemReload";
    public InternalActionRunType RunType => InternalActionRunType.ON_CALL;
    public Task Execute(string[]? args)
    {
        Variables.globalSettings = JsonManager.ConvertFromJson<Settings>(Variables.dataFolder + "Settings.txt").Result;
        return Task.CompletedTask;
    }
}
