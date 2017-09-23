using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using FKTModSettings;
using NPCInfo.UIElements;

namespace NPCInfo
{
	class NPCInfo : Mod
	{
        internal static NPCInfo instance;
        internal bool LoadedFKTModSettings = false;
        internal ModHotKey ToggleHotKeyNPCInfo;
        internal NPCInfoTool npcInfoTool;

        public NPCInfo()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

        public override void Load()
        {
            instance = this;

            UINPCSlot.textures = new Microsoft.Xna.Framework.Graphics.Texture2D[] {
                Main.heartTexture.Resize(22),
                Main.itemTexture[3507].Resize(12),
                Main.EquipPageTexture[0].Resize(12),
                Main.itemTexture[156].Resize(12),
            };

            if (!Main.dedServ)
            {
                ToggleHotKeyNPCInfo = RegisterHotKey("Toggle NPC Info", "X");
                npcInfoTool = new NPCInfoTool();

                Config.LoadConfig();
                LoadedFKTModSettings = ModLoader.GetMod("FKTModSettings") != null;
                try
                {
                    if (LoadedFKTModSettings)
                    {
                        LoadModSettings();
                    }
                }
                catch { }
            }
        }

        public override void PreSaveAndQuit()
        {
            Config.SaveValues();
        }

        public override void PostUpdateInput()
        {
            try
            {
                if (LoadedFKTModSettings && !Main.gameMenu)
                {
                    UpdateModSettings();
                }
            }
            catch { }
        }

        private void LoadModSettings()
        {
            ModSetting setting = ModSettingsAPI.CreateModSettingConfig(this);
            setting.AddBool("isLock", "NPC Info ui position lock", false);
            setting.AddInt("timeOut", "Display time of npc", 5, 60, false);
        }

        private void UpdateModSettings()
        {
            ModSetting setting;
            if (ModSettingsAPI.TryGetModSetting(this, out setting))
            {
                setting.Get("isLock", ref Config.isLock);
                setting.Get("timeOut", ref Config.timeOut);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));

            layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                "NPCInfo: NPC Info",
                delegate
                {
                    npcInfoTool.UIUpdate();
                    npcInfoTool.UIDraw();
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}
