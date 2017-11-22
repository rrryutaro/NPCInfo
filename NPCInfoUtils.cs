using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace NPCInfo
{
    public static class NPCInfoUtils
    {
        public const int tileSize = 16;

        public static Texture2D Resize(this Texture2D texture, int width, int height)
        {
            Texture2D result = texture;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                texture.SaveAsPng(ms, width, height);
                result = Texture2D.FromStream(texture.GraphicsDevice, ms);
            }
            return result;
        }

        public static Texture2D Resize(this Texture2D texture, int size)
        {
            Texture2D result = texture;

            float max = texture.Width < texture.Height ? texture.Height : texture.Width;
            float scale = size / max;
            int width = (int)(texture.Width * scale);
            int height = (int)(texture.Height * scale);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                texture.SaveAsPng(ms, width, height);
                result = Texture2D.FromStream(texture.GraphicsDevice, ms);
            }
            return result;
        }

        public static Vector2 Offset(this Vector2 position, float x, float y)
        {
            position.X += x;
            position.Y += y;
            return position;
        }

		public static TSource FindMin<TSource, TResult>(this IEnumerable<TSource> self, Func<TSource, TResult> selector)
		{
			return self.First(c => selector(c).Equals(self.Min(selector)));
		}

		public static NPC GetActiveNearNPC(int netID)
		{
			NPC result = null;
			try
			{
				result = Main.npc.Where(x => x.active && x.netID == netID).FindMin(x => Vector2.Distance(x.Center, Main.LocalPlayer.Center));
			}
			catch { }
			return result;
		}

		public static Item GetActiveNearItem(int netID)
		{
			Item result = null;
			try
			{
				result = Main.item.Where(x => x.active && x.netID == netID).FindMin(x => Vector2.Distance(x.Center, Main.LocalPlayer.Center));
			}
			catch { }
			return result;
		}

		public static float GetScreenContainsTextPositionX(float defaultX, string text)
		{
			float result = defaultX;
			if (Main.screenWidth < defaultX + Main.fontMouseText.MeasureString(text).X + 6)
				result = (Main.screenWidth - (Main.fontMouseText.MeasureString(text).X + 6));
			return result;
		}

		public static TooltipLine GetPriceTooltipLine(Item item, bool isDisplayPriceText = true)
		{
			TooltipLine result = new TooltipLine(NPCInfo.instance, "Price", string.Empty);

			string[] array = new string[20];
			int num4 = 0;
			int storeValue = item.GetStoreValue();
			int a = (int)Main.mouseTextColor;
			float num20 = (float)Main.mouseTextColor / 255f;

			if (item.shopSpecialCurrency != -1)
			{
				result = new TooltipLine(NPCInfo.instance, "SpecialPrice", string.Empty);
				CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, array, ref num4, storeValue);
				result.text = array[num4];
				result.overrideColor = new Color((int)((byte)(255f * num20)), (int)((byte)(255f * num20)), (int)((byte)(255f * num20)), a);
			}
			else if (storeValue > 0)
			{
				string text = "";
				int num21 = 0;
				int num22 = 0;
				int num23 = 0;
				int num24 = 0;
				int num25 = storeValue * item.stack;
				if (!item.buy)
				{
					num25 = storeValue / 5;
					if (num25 < 1)
					{
						num25 = 1;
					}
					num25 *= item.stack;
				}
				if (num25 < 1)
				{
					num25 = 1;
				}
				if (num25 >= 1000000)
				{
					num21 = num25 / 1000000;
					num25 -= num21 * 1000000;
				}
				if (num25 >= 10000)
				{
					num22 = num25 / 10000;
					num25 -= num22 * 10000;
				}
				if (num25 >= 100)
				{
					num23 = num25 / 100;
					num25 -= num23 * 100;
				}
				if (num25 >= 1)
				{
					num24 = num25;
				}
				if (num21 > 0)
				{
					object obj = text;
					text = string.Concat(new object[] { obj, num21, " ", Lang.inter[15].Value, " " });
				}
				if (num22 > 0)
				{
					object obj = text;
					text = string.Concat(new object[] { obj, num22, " ", Lang.inter[16].Value, " " });
				}
				if (num23 > 0)
				{
					object obj = text;
					text = string.Concat(new object[] { obj, num23, " ", Lang.inter[17].Value, " " });
				}
				if (num24 > 0)
				{
					object obj = text;
					text = string.Concat(new object[] { obj, num24, " ", Lang.inter[18].Value, " " });
				}
				if (!item.buy)
				{
					result.text = (isDisplayPriceText ? Lang.tip[49].Value : "") + " " + text;
				}
				else
				{
					result.text = (isDisplayPriceText ? Lang.tip[50].Value : "") + " " + text;
				}
				if (num21 > 0)
				{
					result.overrideColor = new Color((int)((byte)(220f * num20)), (int)((byte)(220f * num20)), (int)((byte)(198f * num20)), a);
				}
				else if (num22 > 0)
				{
					result.overrideColor = new Color((int)((byte)(224f * num20)), (int)((byte)(201f * num20)), (int)((byte)(92f * num20)), a);
				}
				else if (num23 > 0)
				{
					result.overrideColor = new Color((int)((byte)(181f * num20)), (int)((byte)(192f * num20)), (int)((byte)(193f * num20)), a);
				}
				else if (num24 > 0)
				{
					result.overrideColor = new Color((int)((byte)(246f * num20)), (int)((byte)(138f * num20)), (int)((byte)(96f * num20)), a);
				}
			}
			else if (Main.HoverItem.type != 3817)
			{
				result.text = Lang.tip[51].Value;
				result.overrideColor = new Color((int)((byte)(120f * num20)), (int)((byte)(120f * num20)), (int)((byte)(120f * num20)), a);
			}

			if (result.text == null)
			{
				result.text = string.Empty;
			}
			return result;
		}
	}
}
