using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using System;
using Terraria.ModLoader;

namespace NPCInfo.UIElements
{
    class UIItemSlot : UISlot
    {
        public static int SelectedNetID;
        public Item item;
        public int count;

        public UIItemSlot(int netID, int count)
        {
            item = new Item();
            item.SetDefaults(netID);

            this.count = count;
            sortOrder = netID;

            texture = Main.itemTexture[item.type];

            SetItemFrame(item);
            SetSlotSize();
        }

        public override void Click(UIMouseEvent evt)
        {
            if (SelectedNetID == item.netID)
                SelectedNetID = 0;
            else
                SelectedNetID = item.netID;
        }

        public override void DoubleClick(UIMouseEvent evt)
        {
            if (ModContent.GetInstance<NPCInfoConfig>().isCheatMode)
            {
                var tempItem = NPCInfoUtils.GetActiveNearItem(item.netID);
                if (tempItem != null)
                {
                    Main.LocalPlayer.position = Main.LocalPlayer.position = tempItem.Center.Offset(-Main.LocalPlayer.width / 2, tempItem.height / 2 - Main.LocalPlayer.height);
                    Main.LocalPlayer.fallStart = (int)Main.LocalPlayer.position.Y;
                }
            }
        }

        public override void RightDoubleClick(UIMouseEvent evt)
        {
            if (ModContent.GetInstance<NPCInfoConfig>().isCheatMode)
            {
                var tempItem = NPCInfoUtils.GetActiveNearItem(item.netID);
                if (tempItem != null)
                {
                    tempItem.position = Main.LocalPlayer.position;
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            try
            {
                bool isSelected = SelectedNetID == this.item.netID;
                var tex = backTexture;
                if (isSelected)
                    backTexture = Main.inventoryBack14Texture;
                base.DrawSelf(spriteBatch);
                if (isSelected)
                    backTexture = tex;

                Vector2 pos = base.GetInnerDimensions().Position() + GetCenterPosition(backTexture, texture, 42);
                if (ModContent.GetInstance<NPCInfoConfig>().isAnimation)
                {
                    NextFrame();
                }
                spriteBatch.Draw(texture, pos, new Rectangle?(frame), Color.White, 0, new Vector2(), drawScale, SpriteEffects.None, 0f);
                DrawCount(spriteBatch, count.ToString());

                if (IsMouseHovering)
                {
                    Tool.tooltip = item.Name;
                    NPCInfo.instance.npcInfoTool.targetItem = item;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
        }
    }
}
