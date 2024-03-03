using Discord.WebSocket;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace MusicPlayer.Events;

public class OnLoad: DBEvent
{
    public string Name => "Music Commands";
    public string Description => "The default music commands event loader";
    public async void Start(DiscordSocketClient client)
    {
        var path1    = Functions.dataFolder + "Music/";
        var fileName = path1 + "music_db.json";
        Directory.CreateDirectory(path1);
        Variables._MusicDatabase = new MusicDatabase(fileName);
        await Variables._MusicDatabase.LoadFromFile();
    }
}
