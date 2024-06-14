using Discord.WebSocket;

using DiscordBotCore;
using DiscordBotCore.Database;
using DiscordBotCore.Interfaces;
using static LevelingSystem.Variables;

namespace LevelingSystem;

internal class LevelEvent : DBEvent
{
    public string Name => "Leveling System Event Handler";
    public string Description => "The Leveling System Event Handler";

    public bool RequireOtherThread => false;

    public async void Start(DiscordSocketClient client)
    {

        Directory.CreateDirectory(dataFolder);
        await Task.Delay(200);
        database = new SqlDatabase(dataFolder + "Users.db");
        await database.Open();


        if (!File.Exists(dataFolder + "Settings.txt"))
        {
            globalSettings = new Settings
            {
                SecondsToWaitBetweenMessages = 5,
                MaxEXP                       = 7,
                MinEXP                       = 1
            };
            await DiscordBotCore.Others.JsonManager.SaveToJsonFile(dataFolder + "Settings.txt", globalSettings);
        }
        else
            globalSettings = await DiscordBotCore.Others.JsonManager.ConvertFromJson<Settings>(dataFolder + "Settings.txt");

        if (!await database.TableExistsAsync("Levels"))
            await database.CreateTableAsync("Levels", "UserID VARCHAR(128)", "Level INT", "EXP INT");

        if (!await database.TableExistsAsync("Users"))
            await database.CreateTableAsync("Users", "UserID VARCHAR(128)", "UserMention VARCHAR(128)", "Username VARCHAR(128)", "Discriminator VARCHAR(128)");



        client.MessageReceived += ClientOnMessageReceived;
    }

    private async Task ClientOnMessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot || arg.IsTTS || arg.Content.StartsWith(Application.CurrentApplication.ApplicationEnvironmentVariables["prefix"]))
            return;

        if (waitingList.ContainsKey(arg.Author.Id) && waitingList[arg.Author.Id] > DateTime.Now.AddSeconds(-globalSettings.SecondsToWaitBetweenMessages))
            return;

        var userID = arg.Author.Id.ToString();

        object[] userData = await database.ReadDataArrayAsync($"SELECT * FROM Levels WHERE userID='{userID}'");
        if (userData is null)
        {
            await database.ExecuteAsync($"INSERT INTO Levels (UserID, Level, EXP) VALUES ('{userID}', 1, 0)");
            await database.ExecuteAsync($"INSERT INTO Users (UserID, UserMention) VALUES ('{userID}', '{arg.Author.Mention}')");
            return;
        }

        var level = (int)userData[1];
        var exp   = (int)userData[2];


        var random = new Random().Next(globalSettings.MinEXP, globalSettings.MaxEXP);
        if (exp + random >= level * 8 + 24)
        {
            await database.ExecuteAsync($"UPDATE Levels SET Level={level + 1}, EXP={random - (level * 8 + 24 - exp)} WHERE UserID='{userID}'");
            await arg.Channel.SendMessageAsync($"{arg.Author.Mention} has leveled up to level {level + 1}!");
        }
        else await database.ExecuteAsync($"UPDATE Levels SET EXP={exp + random} WHERE UserID='{userID}'");

        waitingList.Add(arg.Author.Id, DateTime.Now);
    }

}
