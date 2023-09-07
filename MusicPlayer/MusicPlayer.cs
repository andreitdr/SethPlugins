using System.Diagnostics;
using Discord.Audio;
using PluginManager;
using PluginManager.Others;
using PluginManager.Others.Logger;

namespace MusicPlayer;

public class MusicPlayer
{
    private static int defaultByteSize = 1024;
    
    public Queue<MusicInfo> MusicQueue { get; private set; }

    public bool isPaused { get; private set; }
    public bool isPlaying { get; private set; }

    private bool isQueueRunning;
    public int ByteSize { get; private set; }

    public MusicInfo? CurrentlyPlaying { get; private set; }

    public MusicPlayer()
    {
        MusicQueue = new Queue<MusicInfo>();
    }

    public async Task PlayQueue()
    {
        if (isQueueRunning)
        {
            Config.Logger.Log("Another queue is running !", LogLevel.WARNING);
            return;
        }
        
        if (Variables.audioClient is null)
        {
            Config.Logger.Log("Audio Client is null", LogLevel.WARNING);
            return;
        }
        
        isQueueRunning = true;
        
        
        while (MusicQueue.TryDequeue(out MusicInfo? dequeuedMusic))
        {
            CurrentlyPlaying = dequeuedMusic;
            using (var dsAudioStream = Variables.audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                using (var ffmpeg = CreateStream(CurrentlyPlaying.Location))
                {
                    if (ffmpeg is null)
                    {
                        Config.Logger.Log($"Failed to start ffmpeg process. FFMPEG is missing or the {CurrentlyPlaying.Location} has an invalid format.", LogLevel.ERROR);
                        continue;
                    }
                    using (var ffmpegOut = ffmpeg.StandardOutput.BaseStream)
                    {
                        await PlayCurrentTrack(dsAudioStream, ffmpegOut, CurrentlyPlaying.ByteSize ?? defaultByteSize);
                    }
                }
                
            }
        }
        isQueueRunning = false;
        CurrentlyPlaying = null;
    }

    public void Loop(int numberOfTimes)
    {
        if(CurrentlyPlaying is null) return;
        
        Queue<MusicInfo> tempQueue = new Queue<MusicInfo>();
        for (int i = 0; i < numberOfTimes; i++)
        {
            tempQueue.Enqueue(CurrentlyPlaying);
        }
        
        foreach (var musicInfo in MusicQueue)
        {
            tempQueue.Enqueue(musicInfo);
        }
        
        this.MusicQueue = tempQueue;
    }

    public async Task PlayCurrentTrack(Stream DiscordVoiceChannelStream, Stream fileStreamFFMPEG, int byteSize)
    {
        if (isPlaying) return;
        this.ByteSize = byteSize;

        isPlaying = true;
        isPaused = false;

        while (isPlaying)
        {
            if (isPaused) continue;

            var bits = new byte[byteSize];
            var read = await fileStreamFFMPEG.ReadAsync(bits, 0, ByteSize);
            if (read == 0) break;

            try
            {
                await DiscordVoiceChannelStream.WriteAsync(bits, 0, read);
            }
            catch (Exception ex)
            {
                Config.Logger.Error(ex);
                break;
            }
        }
        

        
        await DiscordVoiceChannelStream.FlushAsync();
        await fileStreamFFMPEG.FlushAsync();

        isPlaying = false;
        isPaused = false;
    }

    public void Pause()
    {
        this.isPaused = true;
    }

    public void Unpause()
    {
        this.isPaused = false;
    }

    public bool Enqueue(string musicName)
    {
        MusicInfo? minfo = Variables._MusicDatabase.GetMusicInfo(musicName);
        if (minfo is null) return false;
        
        MusicQueue.Enqueue(minfo);
        return true;
    }

    public void Skip()
    {
        this.isPlaying = false;
    }

    public void SetVolume(float volume)
    {
        // set volume
    }

    private static Process? CreateStream(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg", 
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1", 
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }

    public void Stop()
    {
        this.MusicQueue.Clear();
        this.isPlaying = false;
    }

}