using Discord.WebSocket;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace MusicPlayer.Events;

public class OnLoad : DBEvent
{
    public string Name => "Music Commands";
    public string Description => "The default music commands event loader";
    public void Start(DiscordSocketClient client)
    {
        string path1 = Functions.dataFolder + "Music/";
        string fileName = path1 + "music_db.json";
        Directory.CreateDirectory(path1);
        Variables._MusicDatabase = new MusicDatabase(fileName);
    }
}