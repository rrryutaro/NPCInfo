using System;
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
		public Item targetItem;
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
			targetItem = null;
            base.UIUpdate();
        }

		internal override void UIDraw()
		{
			base.UIDraw();
			if (visible)
			{
				if (NPCInfoUI.instance.ViewMode == ViewMode.CombatNPC && UICombatNPCSlot.SelectedNetID != 0)
				{
					var player = Main.LocalPlayer;
					var nearNPC = Main.npc.Where(x => x.active && x.netID == UICombatNPCSlot.SelectedNetID).FindMin(x => Vector2.Distance(x.position, player.position));
					if (nearNPC != null)
					{
						Utils.DrawLine(Main.spriteBatch, player.Center, nearNPC.Center, Color.Red, Color.Red, 1);
					}
				}
				else if (NPCInfoUI.instance.ViewMode == ViewMode.SpawnNPC && UISpawnNPCSlot.SelectedNetID != 0)
				{
					var player = Main.LocalPlayer;
					var nearNPC = Main.npc.Where(x => x.active && x.netID == UISpawnNPCSlot.SelectedNetID).FindMin(x => Vector2.Distance(x.position, player.position));
					if (nearNPC != null)
					{
						Utils.DrawLine(Main.spriteBatch, player.Center, nearNPC.Center, Color.Red, Color.Red, 1);
					}
				}
				else if (NPCInfoUI.instance.ViewMode == ViewMode.DropItem && UIItemSlot.SelectedNetID != 0)
				{
					var player = Main.LocalPlayer;
					var nearItem = Main.item.Where(x => x.active && x.netID == UIItemSlot.SelectedNetID).FindMin(x => Vector2.Distance(x.position, player.position));
					if (nearItem != null)
					{
						Utils.DrawLine(Main.spriteBatch, player.Center, nearItem.Center, Color.Red, Color.Red, 1);
					}
				}
			}
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
				Vector2 pos = Main.MouseScreen;
				var defaultX = pos.X + 16;
				pos.X = defaultX;

				Main.hoverItemName = tooltip;
				pos.Y += Main.fontMouseText.MeasureString(tooltip).Y + 20;

				//スポーンNPCの価値を表示する（おおよそ）
				if (Config.isDisplaySpawnValue)
				{
					Item item = new Item();
					item.value = (int)targetNPC.value;
					item.buy = true;
					item.stack = 1;
					var price = NPCInfoUtils.GetPriceTooltipLine(item, false);
					pos.X = NPCInfoUtils.GetScreenContainsTextPositionX(defaultX, price.text);
					Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, price.text, pos.X, pos.Y, price.overrideColor ?? Color.White, Color.Black, Vector2.Zero, 1f);
					pos.Y += Main.fontMouseText.MeasureString(price.text).Y + 4;
				}

				string[] texts = { $"{targetNPC.lifeMax}", $"{targetNPC.defDamage}", $"{targetNPC.defDefense}", $"{targetNPC.knockBackResist:0.##}" };
				int maxWidth = UICombatNPCSlot.textures.Sum(x => x.Width) + texts.Sum(x => (int)Main.fontMouseText.MeasureString(x).X) + 46;
				if (Main.screenWidth < pos.X + maxWidth)
					pos.X = Main.screenWidth - maxWidth;

				for (int i = 0; i < UICombatNPCSlot.textures.Length; i++)
				{
					Main.spriteBatch.Draw(UICombatNPCSlot.textures[i], pos, Color.White);
					pos.X += UICombatNPCSlot.textures[i].Width + 4;
					Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, texts[i], pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
					pos.X += Main.fontMouseText.MeasureString(texts[i]).X + 8;
				}

				var nearNPC = NPCInfoUtils.GetActiveNearNPC(targetNPC.netID);
				if (nearNPC != null)
				{
					pos.X = Main.MouseScreen.X + 16;
					pos.Y += UICombatNPCSlot.textures.Max(x => x.Height) + 4;
					var nearPos = (Main.LocalPlayer.Center - nearNPC.Center) / 16;
					var text = $"Near distance: {Math.Abs((int)nearPos.X)} x {Math.Abs((int)nearPos.Y)}";
					pos.X = NPCInfoUtils.GetScreenContainsTextPositionX(defaultX, text);
					Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
				}

			}
			else if (visible && !string.IsNullOrEmpty(tooltip) && NPCInfoUI.instance.ViewMode == ViewMode.DropItem)
			{
				Main.hoverItemName = tooltip;
				if (targetItem != null)
				{
					Vector2 pos = Main.MouseScreen;
					var defaultX = pos.X + 16;
					pos.Y += Main.fontMouseText.MeasureString(tooltip).Y + 20;

					//ドロップアイテムの価値を表示する
					if (Config.isDisplayDropItemValue)
					{
						var price = NPCInfoUtils.GetPriceTooltipLine(targetItem);
						pos.X = NPCInfoUtils.GetScreenContainsTextPositionX(defaultX, price.text);
						Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, price.text, pos.X, pos.Y, price.overrideColor ?? Color.White, Color.Black, Vector2.Zero, 1f);
						pos.Y += Main.fontMouseText.MeasureString(price.text).Y + 4;
					}

					int count = Main.item.Count(x => x.active && x.netID == targetItem.netID);
					string text = $"Count: {count}";
					pos.X = NPCInfoUtils.GetScreenContainsTextPositionX(defaultX, text);
					Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);

					var nearItem = NPCInfoUtils.GetActiveNearItem(targetItem.netID);
					if (nearItem != null)
					{
						pos.Y += Main.fontMouseText.MeasureString(text).Y + 4;
						var nearPos = (Main.LocalPlayer.Center - nearItem.Center) / 16;
						text = $"Near distance: {Math.Abs((int)nearPos.X)} x {Math.Abs((int)nearPos.Y)}";
						pos.X = NPCInfoUtils.GetScreenContainsTextPositionX(defaultX, text);
						Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
					}
				}
			}
			else if (visible && !string.IsNullOrEmpty(tooltip))
			{
				Main.hoverItemName = tooltip;
			}
		}
	}
}
