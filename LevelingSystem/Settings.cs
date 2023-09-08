using PluginManager.Database;
using PluginManager.Others;

namespace LevelingSystem
{
    public class Settings
    {
        public int SecondsToWaitBetweenMessages { get; set; }
        public int MinEXP { get; set; }
        public int MaxEXP { get; set; }
    }

    internal class Variables
    {
        internal static readonly string dataFolder = Functions.dataFolder + "LevelingSystem/";
        internal static SqlDatabase? database;
        internal static Dictionary<ulong, DateTime> waitingList = new();
        internal static Settings globalSettings = new();
    }
}
