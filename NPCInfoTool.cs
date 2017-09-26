using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NPCInfo
{
    class NPCInfoTool : Tool
	{
        public NPC targetNPC;
        public static List<NPCDropInfo> listDropInfo;
        public static Dictionary<string, Dictionary<string, int>> dicModItemNameToType;

        public NPCInfoTool() : base(typeof(NPCInfoUI))
		{
            listDropInfo = null;
            try
            {
                if (File.Exists(NPCInfo.pathNPCDropInfo))
                {
                    listDropInfo = JsonConvert.DeserializeObject<List<NPCDropInfo>>(File.ReadAllText(NPCInfo.pathNPCDropInfo));
                }
            }
            catch { }

            if (listDropInfo == null)
            {
                var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NPCInfo.NPCDropInfo.json");
                if (stream != null)
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        listDropInfo = JsonConvert.DeserializeObject<List<NPCDropInfo>>(sr.ReadToEnd());
                        try
                        {
                            sr.BaseStream.Seek(0, SeekOrigin.Begin);
                            using (StreamWriter sw = new StreamWriter(new FileStream(NPCInfo.pathNPCDropInfo, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
                            {
                                sw.Write(sr.ReadToEnd());
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public void CreateModItemDictionary()
        {
            try
            {
                dicModItemNameToType = new Dictionary<string, Dictionary<string, int>>();
                Dictionary<string, Mod> mods = new Dictionary<string, Mod>();
                foreach (var mod in ModLoader.LoadedMods)
                {
                    mods.Add(mod.Name, mod);
                }
                foreach (var info in listDropInfo)
                {
                    foreach (var item in info.Items)
                    {
                        if (!string.IsNullOrEmpty(item.ModName) && mods.ContainsKey(item.ModName))
                        {
                            var modItem = mods[item.ModName].GetItem(item.ItemName);
                            if (modItem != null)
                            {
                                if (!dicModItemNameToType.ContainsKey(item.ModName))
                                {
                                    dicModItemNameToType.Add(item.ModName, new Dictionary<string, int>());
                                }
                                if (!dicModItemNameToType[item.ModName].ContainsKey(item.ItemName))
                                {
                                    dicModItemNameToType[item.ModName].Add(item.ItemName, modItem.item.type);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        internal override void UIUpdate()
        {
            targetNPC = null;
            base.UIUpdate();
        }

        internal override void TooltipDraw()
        {
            if (visible && Config.isDisplayDropInfo && !string.IsNullOrEmpty(tooltip) && targetNPC != null && listDropInfo != null)
            {
                var dropInfo = listDropInfo.Where(x =>
                {
                    NPCDropInfo result = null;
                    if (targetNPC.modNPC == null)
                    {
                        if (targetNPC.type == x.NPCType)
                            result = x;
                    }
                    else
                    {
                        if (targetNPC.modNPC.mod.Name == x.ModName && targetNPC.FullName == x.NPCName)
                            result = x;
                    }
                    return result != null;
                }).ToArray();

                if (0 < dropInfo.Length)
                {
                    SpriteBatch spriteBatch = Main.spriteBatch;
                    Vector2 pos = Main.MouseScreen;
                    pos.Y += Main.fontMouseText.MeasureString(tooltip).Y + 20;
                    Texture2D texture = null;

                    foreach (var item in dropInfo[0].Items)
                    {
                        try
                        {
                            texture = null;

                            if (string.IsNullOrEmpty(item.ModName))
                            {
                                texture = Main.itemTexture[item.ItemType];
                            }
                            else
                            {
                                if (dicModItemNameToType.ContainsKey(item.ModName) && dicModItemNameToType[item.ModName].ContainsKey(item.ItemName))
                                {
                                    texture = Main.itemTexture[dicModItemNameToType[item.ModName][item.ItemName]];
                                }
                            }
                            if (texture != null)
                            {
                                spriteBatch.Draw(texture, pos, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, 0f);

                                string rate = 5 < item.Chance.ToString().Length ? (100f / item.Chance).ToString("0." + new string('#', item.Chance.ToString().Length - 3)) : (100f / item.Chance).ToString("0.##");

                                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, $"{item.ItemName} ({rate}%)", pos.X + texture.Width + 4, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
                                pos.Y += texture.Height + 4;
                            }
                        }
                        catch { }
                    }
                }

                Main.hoverItemName = tooltip;
            }
        }
    }
}
