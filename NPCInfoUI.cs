using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent.UI.Elements;
using NPCInfo.UIElements;

namespace NPCInfo
{
    class UINPCInfoMainPanel : UIDragablePanel
    {
        public UINPCInfoMainPanel(bool dragable = true, bool resizeableX = false, bool resizeableY = false) : base(dragable, resizeableX, resizeableY)
        {
        }

        public override bool IsLock()
        {
            bool result = Config.isLock;
            return result;
        }
    }

    class NPCInfoUI : UIModState
	{
		static internal NPCInfoUI instance;

		internal UINPCInfoMainPanel panelMain;
        internal UIHoverImageButton closeButton;
        internal UIPanel inlaidPanel;
		internal UIGrid gridNPC;

		internal bool updateNeeded;

        internal string caption = $"NPC Info v{NPCInfo.instance.Version} Count:0";

        private bool showNPCInfo;
		public bool ShowNPCInfo
        {
			get { return showNPCInfo; }
			set
			{
				if (value)
				{
					Append(panelMain);
				}
				else
				{
					RemoveChild(panelMain);
				}
                showNPCInfo = value;

                NPCInfo.instance.npcInfoTool.visible = value;
			}
		}

		public NPCInfoUI(UserInterface ui) : base(ui)
		{
			instance = this;
		}

        public void InitializeUI()
        {
            RemoveAllChildren();

            panelMain = new UINPCInfoMainPanel(true, true, true);
            panelMain.caption = caption;
            panelMain.SetPadding(6);
			panelMain.Left.Set(1300f, 0f);
			panelMain.Top.Set(440f, 0f);
			panelMain.Width.Set(290f, 0f);
			panelMain.MinWidth.Set(100f, 0f);
			panelMain.MaxWidth.Set(1600f, 0f);
			panelMain.Height.Set(290, 0f);
			panelMain.MinHeight.Set(100, 0f);
			panelMain.MaxHeight.Set(900, 0f);

			Texture2D texture = ModLoader.GetMod("NPCInfo").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => ShowNPCInfo = false;
            closeButton.Left.Set(-20f, 1f);
			closeButton.Top.Set(6f, 0f);
			panelMain.Append(closeButton);

			inlaidPanel = new UIPanel();
			inlaidPanel.SetPadding(5);
			inlaidPanel.Top.Pixels = 24;
			inlaidPanel.Width.Set(0, 1f);
			inlaidPanel.Height.Set(-40, 1f);
			panelMain.Append(inlaidPanel);

			gridNPC = new UIGrid();
			gridNPC.Width.Set(-20f, 1f);
			gridNPC.Height.Set(0, 1f);
			gridNPC.ListPadding = 2f;
			inlaidPanel.Append(gridNPC);

			var lootItemsScrollbar = new FixedUIScrollbar(userInterface);
			lootItemsScrollbar.SetView(100f, 1000f);
			lootItemsScrollbar.Height.Set(0, 1f);
			lootItemsScrollbar.Left.Set(-20, 1f);
			inlaidPanel.Append(lootItemsScrollbar);
			gridNPC.SetScrollbar(lootItemsScrollbar);

			updateNeeded = true;
		}

		internal void UpdateGrid()
		{
			if (!updateNeeded) { return; }
			updateNeeded = false;
            InfoCombat.isUpdate = false;

            gridNPC.Clear();

            int sortOrder = 1;
            foreach (var type in InfoCombat.sortedInfo.Values)
			{
                var info = InfoCombat.combatNPC[type];
                info.slot.sortOrder = sortOrder++;
                gridNPC._items.Add(info.slot);
				gridNPC._innerList.Append(info.slot);
			}
			gridNPC.UpdateOrder();
			gridNPC._innerList.Recalculate();

            panelMain.caption = caption.Replace("Count:0", $"Count: { gridNPC._items.Count}");
        }

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
            InfoCombat.Update();
            updateNeeded = InfoCombat.isUpdate;
            UpdateGrid();
		}

        public override TagCompound Save()
        {
            TagCompound result = base.Save();

            if (panelMain != null)
            {
                result.Add("position", panelMain.SavePositionJsonString());
            }
            return result;
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            if (tag.ContainsKey("position"))
            {
                panelMain.LoadPositionJsonString(tag.GetString("position"));
            }
        }
    }
}
