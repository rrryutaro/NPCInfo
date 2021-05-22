using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

namespace NPCInfo.UIElements
{
    public class UISpawnNPCSlot : UISlot
    {
        public static int SelectedNetID;
        public NPC npc;
        public int count;

        public UISpawnNPCSlot(int netID, int count)
        {
            npc = new NPC();
            npc.SetDefaults(netID);
            this.count = count;
            this.sortOrder = netID;

            backTexture = Main.inventoryBack2Texture;
            Main.instance.LoadNPC(npc.type);
            texture = Main.npcTexture[npc.type];

            SetNPCFrame(npc);
            SetSlotSize();
        }

        public override void Click(UIMouseEvent evt)
        {
            if (SelectedNetID == npc.netID)
                SelectedNetID = 0;
            else
                SelectedNetID = npc.netID;
        }

        public override void DoubleClick(UIMouseEvent evt)
        {
            if (ModContent.GetInstance<NPCInfoConfig>().isCheatMode)
            {
                var tempNpc = NPCInfoUtils.GetActiveNearNPC(npc.netID);
                if (tempNpc != null)
                {
                    Main.LocalPlayer.position = tempNpc.Center.Offset(-Main.LocalPlayer.width / 2, tempNpc.height / 2 - Main.LocalPlayer.height);
                    Main.LocalPlayer.fallStart = (int)Main.LocalPlayer.position.Y;
                }
            }
        }

        public override void RightDoubleClick(UIMouseEvent evt)
        {
            if (ModContent.GetInstance<NPCInfoConfig>().isCheatMode)
            {
                var tempNpc = NPCInfoUtils.GetActiveNearNPC(npc.netID);
                if (tempNpc != null)
                {
                    tempNpc.position = Main.LocalPlayer.position;
                }
            }
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
                Vector2 pos = dimensions.Position();
                SetPosition(frame, slotNPCSize, ref pos);
                if (ModContent.GetInstance<NPCInfoConfig>().isAnimation)
                {
                    NextFrame();
                }
                spriteBatch.Draw(texture, pos, new Rectangle?(frame), Color.White, 0, new Vector2(), drawScale, SpriteEffects.None, 0f);
                if (npc.color != default(Color))
                {
                    Main.spriteBatch.Draw(texture, pos, new Rectangle?(frame), npc.color, 0, new Vector2(), drawScale, SpriteEffects.None, 0f);
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
