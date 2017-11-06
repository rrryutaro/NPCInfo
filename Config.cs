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
				config.Get("isCheatMode", ref isCheatMode);
				config.Get("isLock", ref isLock);
                config.Get("timeOut", ref timeOut);
                config.Get("isDisplayDropInfo", ref isDisplayDropInfo);
				config.Get("isDisplaySpawnValue", ref isDisplaySpawnValue);
				config.Get("isDisplayDropItemValue", ref isDisplayDropItemValue);
				config.Get("isDisplayTooltipItemValue", ref isDisplayTooltipItemValue);
				config.Get("isOutputDropInfo", ref isOutputDropInfo);
				config.Get("isAnimation", ref isAnimation);
				config.Get("animationSpeed", ref animationSpeed);

				UIElements.UISlot.frameChangeCount = Config.animationSpeed;
			}
            else
            {
                SaveValues();
            }
        }

        internal static void SaveValues()
        {
            config.Put("version", version);
			config.Put("isCheatMode", isCheatMode);
			config.Put("isLock", isLock);
            config.Put("timeOut", timeOut);
            config.Put("isDisplayDropInfo", isDisplayDropInfo);
			config.Put("isDisplaySpawnValue", isDisplaySpawnValue);
			config.Put("isDisplayDropItemValue", isDisplayDropItemValue);
			config.Put("isDisplayTooltipItemValue", isDisplayTooltipItemValue);
			config.Put("isOutputDropInfo", isOutputDropInfo);
			config.Put("isAnimation", isAnimation);
			config.Put("animationSpeed", animationSpeed);
			config.Save();
        }

		public static bool isCheatMode = false;
		public static bool isLock = false;
        public static int timeOut = 30;
        public static bool isDisplayDropInfo = true;
		public static bool isDisplaySpawnValue = true;
		public static bool isDisplayDropItemValue = true;
		public static bool isDisplayTooltipItemValue = false;
		public static bool isAnimation = false;
		public static int animationSpeed = 10;

		public static bool isOutputDropInfo = false;
    }
}