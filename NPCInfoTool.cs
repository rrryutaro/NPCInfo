using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCInfo.UIElements;

namespace NPCInfo
{
    class NPCInfoTool : Tool
	{
        public NPC targetNPC;
        public static List<NPCDropInfo> listDropInfo;
        public static Dictionary<string, Dictionary<string, int>> dicModItemNameToType;

		private static int[] noRate = { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin };

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
                    foreach (var list in info.list)
                    {
						foreach (var item in list.items)
						{
							if (!string.IsNullOrEmpty(item.mod) && mods.ContainsKey(item.mod))
							{
								var modItem = mods[item.mod].GetItem(item.name);
								if (modItem != null)
								{
									if (!dicModItemNameToType.ContainsKey(item.mod))
									{
										dicModItemNameToType.Add(item.mod, new Dictionary<string, int>());
									}
									if (!dicModItemNameToType[item.mod].ContainsKey(item.name))
									{
										dicModItemNameToType[item.mod].Add(item.name, modItem.item.type);
									}
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
			if (visible && Config.isDisplayDropInfo && !string.IsNullOrEmpty(tooltip) && targetNPC != null && listDropInfo != null && NPCInfoUI.instance.ViewMode == ViewMode.CombatNPC)
			{
                var dropInfo = listDropInfo.Where(x =>
                {
                    NPCDropInfo result = null;
                    if (targetNPC.modNPC == null)
                    {
                        if (targetNPC.type == x.id)
                            result = x;
                    }
                    else
                    {
                        if (targetNPC.modNPC.mod.Name == x.mod && targetNPC.FullName == x.name)
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

                    foreach (var list in dropInfo[0].list)
                    {
						foreach (var item in list.items)
						{
							try
							{
								texture = null;

								if (string.IsNullOrEmpty(item.mod))
								{
									texture = Main.itemTexture[item.id];
								}
								else
								{
									if (dicModItemNameToType.ContainsKey(item.mod) && dicModItemNameToType[item.mod].ContainsKey(item.name))
									{
										texture = Main.itemTexture[dicModItemNameToType[item.mod][item.name]];
									}
								}
								if (texture != null)
								{
									spriteBatch.Draw(texture, pos, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, 0f);

									string name = string.IsNullOrEmpty(item.mod) ? Lang.GetItemName(item.id).Value : item.name;
									if (!noRate.Contains(item.id))
									{
										string rate = 5 < list.rate.ToString().Length ? (100f / list.rate).ToString("0." + new string('#', list.rate.ToString().Length - 3)) : (100f / list.rate).ToString("0.##");
										name += $" ({rate}%)";
									}
									Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, name, pos.X + texture.Width + 4, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
									pos.Y += texture.Height + 4;
								}
							}
							catch { }
						}
                    }
                }

                Main.hoverItemName = tooltip;
            }
			else if (visible && !string.IsNullOrEmpty(tooltip) && targetNPC != null && NPCInfoUI.instance.ViewMode == ViewMode.SpawnNPC)
			{
				Main.hoverItemName = tooltip;
				string[] texts = { $"{targetNPC.lifeMax}", $"{targetNPC.defDamage}", $"{targetNPC.defDefense}", $"{targetNPC.knockBackResist:0.##}" };
				Vector2 pos = Main.MouseScreen;
				pos.X += 16;
				pos.Y += Main.fontMouseText.MeasureString(tooltip).Y + 20;
				int maxWidth = UICombatNPCSlot.textures.Sum(x => x.Width) + texts.Sum(x => (int)Main.fontMouseText.MeasureString(x).X) + 44;
				if (Main.screenWidth < pos.X + maxWidth)
					pos.X = Main.screenWidth - maxWidth;

				for (int i = 0; i < UICombatNPCSlot.textures.Length; i++)
				{
					Main.spriteBatch.Draw(UICombatNPCSlot.textures[i], pos, Color.White);
					pos.X += UICombatNPCSlot.textures[i].Width + 4;
					Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, texts[i], pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
					pos.X += Main.fontMouseText.MeasureString(texts[i]).X + 8;
				}
			}
			else if (visible && !string.IsNullOrEmpty(tooltip))
			{
				Main.hoverItemName = tooltip;
			}
		}
	}
}
