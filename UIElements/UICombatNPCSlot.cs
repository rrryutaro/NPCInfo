using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace NPCInfo.UIElements
{
	public class UICombatNPCSlot : UISlot
	{
		public static int SelectedNetID;
		public static Texture2D[] textures;
		public static float heightSize = 52;
        public static float npcSize = 46;
		public NPC npc;

        public UICombatNPCSlot(NPC npc)
		{
			this.npc = npc;
			backTexture = Main.inventoryBack2Texture;
			Main.instance.LoadNPC(npc.type);
            texture = Main.npcTexture[npc.type];
			SetSlotSize();
		}

		public override void Click(UIMouseEvent evt)
		{
			if (SelectedNetID == npc.netID)
				SelectedNetID = 0;
			else
				SelectedNetID = npc.netID;
		}

		protected override void SetSlotSize()
		{
			this.Width.Set(280, 0f);
			this.Height.Set(heightSize, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
            try
            {
				bool isSelected = SelectedNetID == this.npc.netID;
				var tex = backTexture;
				if (isSelected)
					backTexture = Main.inventoryBack14Texture;
				base.DrawSelf(spriteBatch);
				if (isSelected)
					backTexture = tex;

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

                //名称
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, npc.FullName, dimensions.X + 56, dimensions.Y + 4, Color.White, Color.Black, Vector2.Zero, 1f);

                //ライフ・攻撃力・防御力・ノックバック
                string[] texts = { $"{npc.lifeMax}", $"{npc.defDamage}", $"{npc.defDefense}", $"{npc.knockBackResist:0.##}" };
                pos = new Vector2(dimensions.X + 56, dimensions.Y + 24);
                for (int i = 0; i < textures.Length; i++)
                {
                    Main.spriteBatch.Draw(textures[i], pos, Color.White);
                    pos.X += textures[i].Width + 4;
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, texts[i], pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
                    pos.X += Main.fontMouseText.MeasureString(texts[i]).X + 8;
                }

                if (IsMouseHovering)
                {
                    Tool.tooltip = $"DropItem:";
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
