using System.Security.Cryptography;
using PluginManager;

namespace MusicPlayer;

public class MusicDatabase : Config.Json<string, MusicInfo>
{
    public MusicDatabase(string fileName) : base(fileName)
    {
    }

    public bool ContainsMelodyWithNameOrAlias(string melodyName)
    {
        return (ContainsKey(melodyName) || this.Values.Any(elem => elem.Aliases.Contains(melodyName)));
    }

    public MusicInfo? GetMusicInfo(string alias)
    {
        return this.FirstOrDefault(kvp => kvp.Key == alias || kvp.Value.Aliases.Contains(alias)).Value;
    }

    public void AddNewEntryBasedOnMusicInfo(MusicInfo musicInfo)
    {
        this.Add(musicInfo.Title, musicInfo);
    }
}