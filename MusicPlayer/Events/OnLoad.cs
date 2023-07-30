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
        string rootDir = Functions.dataFolder + "Music/";
        Directory.CreateDirectory(rootDir);
        Variables._MusicDatabase = new MusicDatabase(rootDir + "music_db.json");
    }
}