using PluginManager.Database;
using PluginManager.Others;

namespace LevelingSystem
{
    public class Settings
    {
        public int TimeToWaitBetweenMessages { get; set; }
        public int MinEXP { get; set; }
        public int MaxEXP { get; set; }
    }

    internal class Variables
    {
        internal static readonly string dataFolder = Path.Combine(Functions.dataFolder, "LevelingSystem");
        internal static SqlDatabase? database;
        internal static List<ulong> waitingList = new List<ulong>();
    }
}
