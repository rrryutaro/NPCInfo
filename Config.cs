using Terraria;
using Terraria.IO;

namespace NPCInfo
{
    public static class Config
    {
        private static string ConfigPath = $@"{Main.SavePath}\Mod Configs\NPCInfo.json";
        private static Preferences config;
        private static int version = 1;
        public static void LoadConfig()
        {
            config = new Preferences(ConfigPath);

            if (config.Load())
            {
                config.Get("version", ref version);
                config.Get("isLock", ref isLock);
                config.Get("timeOut", ref timeOut);
            }
            else
            {
                SaveValues();
            }
        }

        internal static void SaveValues()
        {
            config.Put("version", version);
            config.Put("isLock", isLock);
            config.Put("timeOut", timeOut);
            config.Save();
        }

        public static bool isLock = false;
        public static int timeOut = 30;
    }
}