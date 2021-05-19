using Terraria;
using Terraria.IO;

namespace NPCInfo
{
    public static class Config
    {
        private static string ConfigPath = $@"{Main.SavePath}\Mod Configs\NPCInfo.json";

		public static bool isCheatMode = false;
		public static bool isLock = false;
        public static int timeOut = 30;
        public static bool isDisplayDropInfo = true;
		public static bool isDisplaySpawnValue = true;
		public static bool isDisplayDropItemValue = true;
		public static bool isAnimation = false;
		public static int animationSpeed = 10;

		public static bool isOutputDropInfo = false;
    }
}