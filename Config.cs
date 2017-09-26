﻿using Terraria;
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
                config.Get("isDisplayDropInfo", ref isDisplayDropInfo);
                config.Get("isOutputDropInfo", ref isOutputDropInfo);
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
            config.Put("isDisplayDropInfo", isDisplayDropInfo);
            config.Put("isOutputDropInfo", isOutputDropInfo);
            config.Save();
        }

        public static bool isLock = false;
        public static int timeOut = 30;
        public static bool isDisplayDropInfo = true;
        public static bool isOutputDropInfo = false;
    }
}