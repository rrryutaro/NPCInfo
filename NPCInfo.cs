using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using NPCInfo.UIElements;

namespace NPCInfo
{
    class NPCInfo : Mod
    {
        internal static string pathNPCDropInfo = $@"{Main.SavePath}\NPCDropInfo.json";

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

            if (!Main.dedServ)
            {
                // 旧設定ファイルの削除
                var oldConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "NPCInfo.json");
                if (File.Exists(oldConfigPath))
                {
                    File.Delete(oldConfigPath);
                }


                UICombatNPCSlot.textures = new Microsoft.Xna.Framework.Graphics.Texture2D[] {
                    Main.heartTexture.Resize(22),
                    Main.itemTexture[3507].Resize(12),
                    Main.EquipPageTexture[0].Resize(12),
                    Main.itemTexture[156].Resize(12),
                };

                ToggleHotKeyNPCInfo = RegisterHotKey("Toggle NPC Info", "X");
                npcInfoTool = new NPCInfoTool();
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));
            if (layerIndex != -1)
            {
                layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                    "NPCInfo: NPC Info",
                    delegate
                    {
                        try
                        {
                            npcInfoTool.UIUpdate();
                            npcInfoTool.UIDraw();
                        }
                        catch { }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
                layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                    "NPCInfo: Line",
                    delegate
                    {
                        try
                        {
                            npcInfoTool.UIDrawLine();
                        }
                        catch { }
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }

            layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (layerIndex != -1)
            {
                layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                    "NPCInfo: Tooltip",
                    delegate
                    {
                        try
                        {
                            npcInfoTool.TooltipDraw();
                        }
                        catch { }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void AddRecipes()
        {
            if (!Main.dedServ)
            {
                npcInfoTool.CreateModItemDictionary();
            }
        }
    }

}
