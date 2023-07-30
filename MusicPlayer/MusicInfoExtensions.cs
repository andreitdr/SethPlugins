using System.Runtime.CompilerServices;
using PluginManager.Others;

namespace MusicPlayer;

public static class MusicInfoExtensions
{
    public static void AddAlias(this MusicInfo musicInfo, string alias)
    {
        musicInfo.Aliases.Add(alias);
    }

    public static void RemoveAlias(this MusicInfo musicInfo, string alias)
    {
        musicInfo.Aliases.Remove(alias);
    }

    public static MusicInfo CreateMusicInfo(string title, string fileLocation, string? Description = "Unknown", List<string>? aliases = null, int? byteSize = 1024)
    {
        return new MusicInfo()
        {
            Title = title, Aliases = aliases, Description = Description, Location = fileLocation, ByteSize = byteSize
        };
    }
}