using Discord;
using Discord.Audio;
using Discord.WebSocket;
using PluginManager;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace MusicPlayer.SlashCommands;

public class Play : DBSlashCommand
{
    public string Name => "play";
    public string Description => "Play music command";
    public bool canUseDM => false;

    public List<SlashCommandOptionBuilder> Options => new List<SlashCommandOptionBuilder>()
    {
        new SlashCommandOptionBuilder()
        {
            IsRequired = true, Description = "The music name to be played", Name = "music-name",
            Type = ApplicationCommandOptionType.String
        }
    };
    
    public async void ExecuteServer(SocketSlashCommand context)
    {
        string? melodyName = context.Data.Options.First().Value as string;

        if (melodyName is null)
        {
            await context.RespondAsync("Failed to retrieve melody with name " + melodyName);
            return;
        }
        
        var melody = Variables._MusicDatabase.GetMusicInfo(melodyName);
        if (melody is null)
        {
            await context.RespondAsync("The searched melody does not exists in the database. Sorry :(");
            return;
        }

        IGuildUser? user = context.User as IGuildUser;
        if (user is null)
        {
            await context.RespondAsync("Failed to get user data from channel ! Check error log at " + DateTime.Now.ToLongTimeString());
            Config.Logger.Log("User is null while trying to convert from context.User to IGuildUser.", LogLevel.ERROR);
            return;
        }

        IVoiceChannel? voiceChannel = user.VoiceChannel;
        if (voiceChannel is null)
        {
            await context.RespondAsync("Unknown voice channel. Maybe I do not have permission to join it ?");
            return;
        }

        if (Variables.audioClient is null)
        {
            Variables.audioClient = await voiceChannel.ConnectAsync(true); // self deaf
        }
        
        Variables._MusicPlayer ??= new MusicPlayer();

        if (!Variables._MusicPlayer.Enqueue(melodyName))
        {
            await context.RespondAsync("Failed to enqueue your request. Something went wrong !");
            return;
        }
        await context.RespondAsync("Enqueued your request");

        await Variables._MusicPlayer.PlayQueue(); //start queue
    }
}