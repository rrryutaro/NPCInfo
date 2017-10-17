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
	public class UISlot : UIElement
	{
		public static float scale = 1.0f;
		public Texture2D backTexture = Main.inventoryBack9Texture;
        public Texture2D texture;
		public int sortOrder;
		public int size;

		public UISlot()
		{
		}

		public UISlot(Texture2D texture)
		{
            this.texture = texture;
            SetSlotSize();
        }

        protected virtual void SetSlotSize()
        {
            int size = Math.Max(backTexture.Width, backTexture.Height);
            this.Width.Set(size * scale, 0f);
            this.Height.Set(size * scale, 0f);
        }

        public override void Recalculate()
        {
            SetSlotSize();
            base.Recalculate();
        }

        public override int CompareTo(object obj)
        {
            int result = sortOrder < (obj as UISlot).sortOrder ? -1 : 1;
            return result;
        }

		protected Vector2 GetCenterPosition(Texture2D back, Texture2D front, float size)
		{
			Vector2 result;
			float scale = 1f;
			float frontMax = Math.Max(front.Width, front.Height);
			if (size < frontMax)
			{
				scale = size / frontMax;
			}
			result = new Vector2(back.Width / 2 - (front.Width * scale) / 2, back.Height / 2 - (front.Width * scale) / 2);
			return result;
		}

		protected void SetPosition(Rectangle rect, float size, ref Vector2 pos)
		{
			float scale = 1f;
			if (size < rect.Width || size < rect.Height)
			{
				scale = size / (float)(rect.Width > rect.Height ? rect.Width : rect.Height);
			}
			pos.X += backTexture.Width / 2 - (rect.Width * scale) / 2;
			pos.Y += backTexture.Height / 2 - (rect.Height * scale) / 2;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
        {
			try
			{
				CalculatedStyle dimensions = base.GetInnerDimensions();
				Rectangle rectangle = dimensions.ToRectangle();
				spriteBatch.Draw(backTexture, dimensions.Position(), null, Color.White);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Write(ex.Message);
			}

		}
		protected virtual void DrawCount(SpriteBatch spriteBatch, string count)
		{
			try
			{
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, count, base.GetInnerDimensions().Position() + new Vector2(6f, 26f) * scale, Color.Wheat, 0f, Vector2.Zero, new Vector2(scale));
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Write(ex.Message);
			}
		}
	}
}
