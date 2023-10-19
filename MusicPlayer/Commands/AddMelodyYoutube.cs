using System.Diagnostics;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace MusicPlayer.Commands;

public class AddMelodyYoutube : DBCommand
{
    public string Command => "add_melody_youtube";
    public List<string>? Aliases => new () { "madd-yt" };
    public string Description => "Add melody to the database from a youtube link";
    public string Usage => "add_melody_youtube [URL] <alias1|alias2|...>";
    public bool requireAdmin => true;

    public async void ExecuteServer(DBCommandExecutingArguments args)
    {
        if (args.arguments is null)
        {
            await args.context.Channel.SendMessageAsync("Invalid arguments given. Please use the following format:\nadd_melody_youtube [URL]");
            return;
        }
        
        
        string URL = args.arguments[0];
        
        if (!URL.StartsWith("https://www.youtube.com/watch?v=") && !URL.StartsWith("https://youtu.be/"))
        {
            await args.context.Channel.SendMessageAsync("Invalid URL given. Please use the following format:\nadd_melody_youtube [URL]");
            return;
        }
        
        var msg = await args.context.Channel.SendMessageAsync("Saving melody ...");
        
        string? title = await DownloadMelody(URL);
        
        if (title == null)
        {
            await msg.ModifyAsync(x => x.Content = "Failed to download melody !");
            return;
        }
        
        List<string>? aliases = null;
        if (args.arguments.Length > 1)
        {
            string joinedAliases = string.Join(" ", args.arguments.Skip(1));
            aliases = joinedAliases.Split('|').ToList();
        }
        else 
            aliases = new List<string>() { title.Split(' ')[0] };


        if (Variables._MusicDatabase.ContainsMelodyWithNameOrAlias(title))
            Variables._MusicDatabase.Remove(title);
        
        Variables._MusicDatabase.Add(title, new MusicInfo() {
            Aliases = aliases, 
            ByteSize = 1024,
            Description = "Melody added from youtube link", 
            Location = $"{Functions.dataFolder}Music/Melodies/{title}.mp3", 
            Title = title
        });
        
        

        await Variables._MusicDatabase.SaveToFile();
        await msg.ModifyAsync(x => x.Content = "Melody saved !");
    }

    private async Task<string?> DownloadMelody(string url)
    {
        Console.WriteLine("Downloading melody: " + url);
        Process process = new Process();
        process.StartInfo.FileName = "./yt-dlp";
        process.StartInfo.Arguments = $"-x --force-overwrites -o \"{Functions.dataFolder}Music/Melodies/%(title)s.%(ext)s\" --audio-format mp3 {url}";
        
        process.StartInfo.RedirectStandardOutput = true;
        string title = "";
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                if (args.Data.StartsWith("[ExtractAudio] Destination: "))
                {
                    title = args.Data.Replace("[ExtractAudio] Destination: ", "").Replace(".mp3", "");
                    title = title.Split('/').Last().Replace(".mp3", "").TrimEnd();

                    Console.WriteLine("Output title: " + title);
                }
            }
        };

        process.Start();
        Console.WriteLine("Waiting for process to exit ...");
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        
        return title ?? null;
    }
}