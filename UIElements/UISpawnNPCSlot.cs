using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.UI.Chat;

namespace NPCInfo.UIElements
{
	public class UISpawnNPCSlot : UISlot
	{
		public NPC npc;
		public int count;
		public static float npcSize = 46;

		private static int[] multiWidthFrame = {564, 565, 576, 577 };

		public UISpawnNPCSlot(int netID, int count)
		{
			npc = new NPC();
			npc.SetDefaults(netID);
			if (multiWidthFrame.Contains(netID))
				npc.frame.Width = texture.Width / 5;

			this.count = count;
			this.sortOrder = netID;

			backTexture = Main.inventoryBack2Texture;
			Main.instance.LoadNPC(npc.type);
            texture = Main.npcTexture[npc.type];
			SetSlotSize();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
		{
            try
            {
				base.DrawSelf(spriteBatch);

				CalculatedStyle dimensions = base.GetInnerDimensions();
				float scale = 1f;
				if (npcSize < npc.frame.Width || npcSize < npc.frame.Height)
				{
					scale = npcSize / (float)(npc.frame.Width > npc.frame.Height ? npc.frame.Width : npc.frame.Height);
				}
				Vector2 pos = dimensions.Position();
				SetPosition(npc.frame, npcSize, ref pos);
				spriteBatch.Draw(texture, pos, new Rectangle?(npc.frame), Color.White, 0, new Vector2(), scale, SpriteEffects.None, 0f);
				if (npc.color != default(Color))
				{
					Main.spriteBatch.Draw(texture, pos, new Rectangle?(npc.frame), npc.color, 0, new Vector2(), scale, SpriteEffects.None, 0f);
				}
				DrawCount(spriteBatch, count.ToString());

				if (IsMouseHovering)
				{
					Tool.tooltip = npc.FullName;
					NPCInfo.instance.npcInfoTool.targetNPC = npc;
				}
			}
			catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
        }
	}
}
