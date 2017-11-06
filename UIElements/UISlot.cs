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
		public static int frameChangeCount = 5;
		public static float slotNPCSize = 46;
		public static float slotItemSize = 42;

		public Texture2D backTexture = Main.inventoryBack9Texture;
        public Texture2D texture;
		public int sortOrder;
		public int size;

		protected static int[] multiWidthFrame = { 564, 565, 576, 577 };
		protected Rectangle frame;
		protected float drawScale = 1f;
		protected int frameChangeIndex;
		protected int frameWidthCount;
		protected int frameWidthIndex;
		protected int frameHeightCount;
		protected int frameHeightIndex;
		protected int frameCount;
		protected int frameIndex;

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

		protected virtual void SetNPCFrame(NPC npc)
		{
			frame = npc.frame;
			frameHeightCount = Main.npcFrameCount[npc.type];
			if (multiWidthFrame.Contains(npc.netID))
			{
				frameWidthCount = 5;
				frame.Width = texture.Width / 5;
				switch (npc.netID)
				{
					case 564:
					case 565:
						frameCount = 41;
						break;
					case 576:
					case 577:
						frameCount = 48;
						break;
					default:
						frameCount = frameHeightCount * frameWidthCount;
						break;
				}
			}
			else
			{
				frameWidthCount = 1;
				frameCount = frameHeightCount;
			}
			if (slotNPCSize < frame.Width || slotNPCSize < frame.Height)
			{
				drawScale = slotNPCSize / Math.Max(frame.Width, frame.Height);
			}
		}

		protected virtual void SetItemFrame(Item item)
		{
			frame.Width = texture.Width;
			frameWidthCount = 1;
			if (Main.itemAnimations[item.type] != null)
			{
				frame.Height = texture.Height / Main.itemAnimations[item.type].FrameCount;
				frameCount = frameHeightCount = Main.itemAnimations[item.type].FrameCount;
			}
			else
			{
				frame.Height = texture.Height;
				frameCount = frameHeightCount = 1;
			}
			if (slotItemSize < frame.Width || slotItemSize < frame.Height)
			{
				drawScale = slotItemSize / Math.Max(frame.Width, frame.Height);
			}
		}

		protected virtual void NextFrame()
		{
			if (frameChangeCount < ++frameChangeIndex)
			{
				frameChangeIndex = 0;
				frame.X = (texture.Width / frameWidthCount) * frameWidthIndex;
				frame.Y = (texture.Height / frameHeightCount) * frameHeightIndex;

				++frameIndex;
				if (frameHeightCount <= ++frameHeightIndex)
				{
					frameHeightIndex = 0;
					if (frameWidthCount <= ++frameWidthIndex)
					{
						frameWidthIndex = 0;
						frameIndex = 0;
					}
				}
				else if (frameCount <= frameIndex)
				{
					frameIndex = 0;
					frameHeightIndex = 0;
					frameWidthIndex = 0;
				}
			}
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
