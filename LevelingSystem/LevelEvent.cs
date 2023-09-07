using Discord.WebSocket;

using PluginManager;
using PluginManager.Database;
using PluginManager.Interfaces;
using PluginManager.Others;

namespace LevelingSystem
{
    internal class LevelEvent : DBEvent
    {
        public string Name => "Leveling System Event Handler";
        public string Description => "The Leveling System Event Handler";

        internal static Settings globalSettings = new();


        public async void Start(DiscordSocketClient client)
        {

            Directory.CreateDirectory(Variables.dataFolder);
            await Task.Delay(200);
            Variables.database = new SqlDatabase(Variables.dataFolder + "/Users.dat");
            await Variables.database.Open();


            if (!File.Exists(Variables.dataFolder + "Settings.txt"))
            {
                globalSettings = new Settings { TimeToWaitBetweenMessages = 5, MaxEXP = 7, MinEXP = 1 };
                await JsonManager.SaveToJsonFile<Settings>(Variables.dataFolder + "Settings.txt", globalSettings);
            }
            else
                globalSettings = await JsonManager.ConvertFromJson<Settings>(Variables.dataFolder + "Settings.txt");

            if (!await Variables.database.TableExistsAsync("Levels"))
                await Variables.database.CreateTableAsync("Levels", "UserID VARCHAR(128)", "Level INT", "EXP INT");

            if (!await Variables.database.TableExistsAsync("Users"))
                await Variables.database.CreateTableAsync("Users", "UserID VARCHAR(128)", "UserMention VARCHAR(128)", "Username VARCHAR(128)", "Discriminator VARCHAR(128)");



            client.MessageReceived += ClientOnMessageReceived;
        }

        private async Task ClientOnMessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot || arg.IsTTS || arg.Content.StartsWith(Config.AppSettings["prefix"]) || Variables.waitingList.Contains(arg.Author.Id)) return;
            string userID = arg.Author.Id.ToString();

            object[] userData = await Variables.database.ReadDataArrayAsync($"SELECT * FROM Levels WHERE userID='{userID}'");
            if (userData is null)
            {
                await Variables.database.ExecuteAsync($"INSERT INTO Levels (UserID, Level, EXP) VALUES ('{userID}', 1, 0)");
                await Variables.database.ExecuteAsync($"INSERT INTO Users (UserID, UserMention) VALUES ('{userID}', '{arg.Author.Mention}')");
                return;
            }

            int level = (int)userData[1];
            int exp = (int)userData[2];


            int random = new Random().Next(globalSettings.MinEXP, globalSettings.MaxEXP);
            if (exp + random >= level * 8 + 24)
            {
                await Variables.database.ExecuteAsync($"UPDATE Levels SET Level={level + 1}, EXP={random - (level * 8 + 24 - exp)} WHERE UserID='{userID}'");
                await arg.Channel.SendMessageAsync($"{arg.Author.Mention} has leveled up to level {level + 1}!");
            }
            else await Variables.database.ExecuteAsync($"UPDATE Levels SET EXP={exp + random} WHERE UserID='{userID}'");

            Thread t = new Thread(() =>
            {
                Thread.Sleep(globalSettings.TimeToWaitBetweenMessages * 1000);
                Variables.waitingList.Add(arg.Author.Id);
            });

            t.Start();

        }

    }
}