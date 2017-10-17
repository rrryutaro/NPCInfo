using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using System.Text;
using Terraria.UI.Chat;

namespace NPCInfo.UIElements
{
	class UIItemSlot : UISlot
	{
		public Item item;
		public int count;

        public UIItemSlot(int netID, int count)
		{
			item = new Item();
			item.SetDefaults(netID);

			this.count = count;
			sortOrder = netID;

			texture = Main.itemTexture[item.type];
			float scale = 1f;
			float max = Math.Max(texture.Width, texture.Height);
			if (42 < max)
			{
				scale = 42 / max;
			}
			texture = texture.Resize((int)(texture.Width * scale), (int)(texture.Height * scale));

			SetSlotSize();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			try
			{
				base.DrawSelf(spriteBatch);

				Vector2 pos = base.GetInnerDimensions().Position() + GetCenterPosition(backTexture, texture, 42);
				spriteBatch.Draw(texture, pos, Color.White);
				DrawCount(spriteBatch, count.ToString());

				if (IsMouseHovering)
				{
					Tool.tooltip = item.Name;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Write(ex.Message);
			}
		}
	}
}
