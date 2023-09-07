using System.Runtime.CompilerServices;
using Discord;
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
    
    public static Embed ToEmbed(this MusicInfo musicInfo, Color? embedColor = null)
    {
        EmbedBuilder builder = new EmbedBuilder();
        builder.Color = embedColor ?? Color.Default;
        builder.WithTitle("Title: " + musicInfo.Title);
        builder.WithDescription("Description: " + musicInfo.Description);
        builder.AddField("Aliases", string.Join(", ", musicInfo.Aliases));
        builder.AddField("Location", musicInfo.Location);
        builder.AddField("ByteSize", musicInfo.ByteSize);
        return builder.Build();
    }
    
    public static Embed ToEmbed(this List<MusicInfo> musicInfo, Color? embedColor = null)
    {
        EmbedBuilder builder = new EmbedBuilder();
        builder.Color = embedColor ?? Color.Default;
        builder.WithTitle("Search results");
        builder.WithDescription("Found " + musicInfo.Count + " results");
        builder.AddField("Results", string.Join("\n", musicInfo.Select(item => item.Title)));
        return builder.Build();
    }
}