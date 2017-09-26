using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace NPCInfo.UIElements
{
	public class UINPCSlot : UIElement
	{
        public static Texture2D backTexture = Main.inventoryBack2Texture;
        public static Texture2D[] textures;
        public static float heightSize = 52;
        public static float npcSize = 46;

        public Texture2D npcTexture;
		internal float scale = .75f;
		public NPC npc;
        public int sortOrder;

        public UINPCSlot(NPC npc, float scale = .75f)
		{
			this.scale = scale;
			this.npc = npc;

            Main.instance.LoadNPC(npc.type);
            npcTexture = Main.npcTexture[npc.type];
            this.Width.Set(280, 0f);
            this.Height.Set(heightSize, 0f);
        }

        public void SetPosition(Rectangle rect, float size, ref Vector2 pos)
        {
            float scale = 1f;
            if (size < rect.Width || size < rect.Height)
            {
                scale = size / (float)(rect.Width > rect.Height ? rect.Width : rect.Height);
            }
            pos.X += backTexture.Width / 2 - (rect.Width * scale) / 2;
            pos.Y += backTexture.Height / 2 - (rect.Height * scale) / 2;
        }

        public override int CompareTo(object obj)
        {
            int result = sortOrder < (obj as UINPCSlot).sortOrder ? 1 : -1;
            return result;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
		{
            try
            {
                CalculatedStyle dimensions = base.GetInnerDimensions();

                spriteBatch.Draw(backTexture, dimensions.Position(), null, Color.White);

                float scale = 1f;
                if (npcSize < npc.frame.Width || npcSize < npc.frame.Height)
                {
                    scale = npcSize / (float)(npc.frame.Width > npc.frame.Height ? npc.frame.Width : npc.frame.Height);
                }
                Vector2 pos = dimensions.Position();
                SetPosition(npc.frame, npcSize, ref pos);

                spriteBatch.Draw(npcTexture, pos, new Rectangle?(npc.frame), Color.White, 0, new Vector2(), scale, SpriteEffects.None, 0f);
                if (npc.color != default(Microsoft.Xna.Framework.Color))
                {
                    Main.spriteBatch.Draw(npcTexture, pos, new Rectangle?(npc.frame), npc.color, 0, new Vector2(), scale, SpriteEffects.None, 0f);
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
