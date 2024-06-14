using DiscordBotCore.Interfaces;
using DiscordBotCore.Others;
using DiscordBotCore.Others.Actions;

namespace LevelingSystem;

public class ReloadAction: ICommandAction
{
    public string ActionName => "LevelingSystemReload";
    public string? Description => "Reloads the Leveling System config file";
    public string? Usage => "LevelingSystemReload";
    public InternalActionRunType RunType => InternalActionRunType.ON_CALL;

    public IEnumerable<InternalActionOption> ListOfOptions => [];

    public async Task Execute(string[]? args)
    {
        Variables.globalSettings = await JsonManager.ConvertFromJson<Settings>(Variables.dataFolder + "Settings.txt");
    }
}
