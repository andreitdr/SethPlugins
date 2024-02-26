using System.Diagnostics;
using PluginManager.Others;

namespace MusicPlayer;

public class YoutubeDLP
{
    public static async Task<string?> DownloadMelody(string url)
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
                    title = title.Replace("\\", "/");
                    title = title.Split('/').Last().Replace(".mp3", "").TrimEnd();

                    Console.WriteLine("Output title: " + title);

                    return;
                }
                
                Console.WriteLine(args.Data);
            }
        };

        process.Start();
        Console.WriteLine("Waiting for process to exit ...");
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        
        return title ?? null;
    }
}