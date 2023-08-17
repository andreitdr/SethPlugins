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
        string arguments = string.Join(" ", args.arguments);
        string[] split = arguments.Split(',');
        
        if (split.Length < 4)
        {
            string message = "";
            message += "Invalid arguments given. Please use the following format:\n";
            message += "add_melody [title],[description?],[aliases],[byteSize]\n";
            message += "title: The title of the melody\n";
            message += "description: The description of the melody\n";
            message += "aliases: The aliases of the melody. Use | to separate them\n";
            message += "byteSize: The byte size of the melody. Default is 1024. ( & will use default)\n";

            await args.context.Channel.SendMessageAsync(message);

            return;
        }
        
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
        

        string title = split[0];
        string description = split[1];
        string[]? aliases = split[2]?.Split('|') ?? null;
        string byteSize = split[3];
        int bsize;
        if (!int.TryParse(byteSize, out bsize))
            bsize = 1024;
        

        var msg = await args.context.Channel.SendMessageAsync("Saving melody ...");
        Console.WriteLine("Saving melody");

        IProgress<float> downloadProgress = new Progress<float>();

        string location = Functions.dataFolder + $"Music/Melodies/{title}.mp3";
        Directory.CreateDirectory(Functions.dataFolder + "Music/Melodies");
        await ServerCom.DownloadFileAsync(file.Url,location , downloadProgress);
        
        Console.WriteLine($"Done. Saved at {location}");

        await msg.ModifyAsync(a => a.Content = "Done");


        MusicInfo info =
            MusicInfoExtensions.CreateMusicInfo(title, location, description ?? "Unknown", aliases.ToList(), bsize);
        
        Variables._MusicDatabase?.Add(title, info);

        EmbedBuilder builder = new EmbedBuilder();
        builder.Title = "A new music was successfully added !";
        builder.AddField("Title", info.Title);
        builder.AddField("Description", info.Description);
        builder.AddField("Aliases", string.Join(" | ", aliases));
        await args.context.Channel.SendMessageAsync(embed: builder.Build());

        await Variables._MusicDatabase.SaveToFile();

    }
}