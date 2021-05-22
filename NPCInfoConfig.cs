using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace NPCInfo
{
    [Label("Config")]
    public class NPCInfoConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Cheat mode")]
        [DefaultValue(false)]
        public bool isCheatMode;

        [Label("NPC Info ui position lock")]
        [DefaultValue(false)]
        public bool isUiLock;

        [Label("Display time of npc")]
        [DefaultValue(30)]
        public int timeOut;

        [Label("Display drop info")]
        [DefaultValue(true)]
        public bool isDisplayDropInfo = true;

        [Label("Display spawn npc value")]
        [DefaultValue(true)]
        public bool isDisplaySpawnValue = true;

        [Label("Display drop item value")]
        [DefaultValue(true)]
        public bool isDisplayDropItemValue = true;

        [Label("Animation")]
        [DefaultValue(true)]
        public bool isAnimation = false;

        [Label("Animation speed")]
        [DefaultValue(10)]
        public int animationSpeed = 10;

    }
}
