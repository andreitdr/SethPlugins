using Discord;
using PluginManager.Interfaces;
using PluginManager.Online;
using PluginManager.Others;

namespace MusicPlayer.Commands;

public class CreateMelody : DBCommand
{
    public string Command => "add_melody";
    public List<string>? Aliases => new List<string>() { "madd" };
    public string Description => "Add a custom melody to the database";
    public string Usage => "add_melody [title],[description?],[aliases],[byteSize]";
    public bool requireAdmin => false;
    
    public async void ExecuteServer(DBCommandExecutingArguments args)
    {
        
        Console.WriteLine(args.cleanContent);
        string arguments = string.Join(" ", args.arguments);
        string[] split = arguments.Split(',');

        string title = split[0];
        string description = split[1];
        string[] aliases = split[2].Split(' ');
        string byteSize = split[3];
        int bsize;
        if (!int.TryParse(byteSize, out bsize))
            bsize = 1024;

        if (args.context.Message.Attachments.Count == 0)
        {
            await args.context.Channel.SendMessageAsync("You must upload a valid .mp3 audio or .mp4 video file !!");
            return;
        }

        var file = args.context.Message.Attachments.FirstOrDefault();
        if (!(file.Filename.EndsWith(".mp3") || file.Filename.EndsWith(".mp4")))
        {
            await args.context.Channel.SendMessageAsync("Invalid file format !!");
            return;
        }

        var msg = await args.context.Channel.SendMessageAsync("Saving melody ...");
        Console.WriteLine("Saving melody");
        
        IProgress<float> downloadProgress = new Progress<float>(async (prog) =>
        {
            await msg.ModifyAsync(prop => prop.Content += ".");
        });

        string location = Functions.dataFolder + $"Music/Melodies/{title}.mp3";
        Directory.CreateDirectory(Functions.dataFolder + "Music/melodies");
        await ServerCom.DownloadFileAsync(file.Url,location , downloadProgress);
        
        Console.WriteLine($"Done. Saved at {location}");


        MusicInfo info =
            MusicInfoExtensions.CreateMusicInfo(title, location, description ?? "Unknown", aliases.ToList(), bsize);
        
        Variables._MusicDatabase.Add(title, info);

        EmbedBuilder builder = new EmbedBuilder();
        builder.Title = "A new music was successfully added !";
        builder.AddField("Title", info.Title);
        builder.AddField("Description", info.Description);
        builder.AddField("Aliases", string.Join(" | ", aliases));
        await args.context.Channel.SendMessageAsync(embed: builder.Build());

        await Variables._MusicDatabase.Save();

    }
}