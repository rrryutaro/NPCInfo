using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
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
		internal UIImageListButton btnViewMode;
		internal UIImageListButton btnSpawnNPCFilter;

		internal bool updateNeeded;

        internal string caption = $"NPC Info v{NPCInfo.instance.Version} Count:0";

		static internal int menuIconSize = 28;
		static internal int menuMargin = 4;

		private Dictionary<int, int> dicSpawnNPC = new Dictionary<int, int>();
		private Dictionary<int, int> dicDropItem = new Dictionary<int, int>();

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
            panelMain.Left.Set(400f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(290f, 0f);
			panelMain.MinWidth.Set(290f, 0f);
			panelMain.MaxWidth.Set(Main.screenWidth, 0f);
			panelMain.Height.Set(298, 0f);
			panelMain.MinHeight.Set(124, 0f);
			panelMain.MaxHeight.Set(Main.screenHeight, 0f);

			Texture2D texture = ModLoader.GetMod("NPCInfo").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => ShowNPCInfo = false;
            closeButton.Left.Set(-20f, 1f);
			closeButton.Top.Set(6f, 0f);
			panelMain.Append(closeButton);

			inlaidPanel = new UIPanel();
			inlaidPanel.SetPadding(5);
			inlaidPanel.Top.Pixels = 32;
			inlaidPanel.Width.Set(0, 1f);
			inlaidPanel.Height.Set(-50, 1f);
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

			int leftPos = menuMargin;
			btnViewMode = new UIImageListButton(
				new List<Texture2D>() {
					Main.itemTexture[ItemID.AlphabetStatue1].Resize(menuIconSize),
					Main.itemTexture[ItemID.AlphabetStatue2].Resize(menuIconSize),
					Main.itemTexture[ItemID.AlphabetStatue3].Resize(menuIconSize)},
				new List<object>() { ViewMode.CombatNPC, ViewMode.SpawnNPC, ViewMode.DropItem },
				new List<string>() { "Combat NPC", "Spawn NPC", "Drop Item" });
			btnViewMode.OnClick += (a, b) =>
			{
				btnViewMode.NextIamge();
				ChangeViewMode();
			};
			btnViewMode.OnRightClick += (a, b) =>
			{
				btnViewMode.PrevIamge();
				ChangeViewMode();
			};
			btnViewMode.Left.Set(leftPos, 0f);
			btnViewMode.Top.Set(0f, 0f);
			panelMain.Append(btnViewMode);

			btnSpawnNPCFilter = new UIImageListButton(
				new List<Texture2D>() {
					Main.itemTexture[ItemID.AlphabetStatue1].Resize(menuIconSize),
					Main.itemTexture[ItemID.AlphabetStatue2].Resize(menuIconSize)},
				new List<object>() { SpawnNPCFilter.All, SpawnNPCFilter.HideTownNPC },
				new List<string>() { "Spawn NPC: All", "Spawn NPC: Hide twon NPC" });
			btnSpawnNPCFilter.OnClick += (a, b) =>
			{
				btnSpawnNPCFilter.NextIamge();
				ChangeSpawnNPCFilter();
			};
			btnSpawnNPCFilter.OnRightClick += (a, b) =>
			{
				btnSpawnNPCFilter.PrevIamge();
				ChangeSpawnNPCFilter();
			};
			leftPos += menuIconSize + menuMargin;
			btnSpawnNPCFilter.Left.Set(leftPos, 0f);
			btnSpawnNPCFilter.Top.Set(0f, 0f);

			updateNeeded = true;
		}

		private void ChangeViewMode()
		{
			gridNPC.Clear();
			updateNeeded = true;
			InfoCombat.isUpdate = true;
			if (ViewMode == ViewMode.SpawnNPC)
				panelMain.Append(btnSpawnNPCFilter);
			else
				panelMain.RemoveChild(btnSpawnNPCFilter);
		}

		private void ChangeSpawnNPCFilter()
		{
			gridNPC.Clear();
			dicSpawnNPC.Clear();
			updateNeeded = true;
		}

		internal void UpdateGrid()
		{
			if (!updateNeeded) { return; }
			updateNeeded = false;
            InfoCombat.isUpdate = false;

            gridNPC.Clear();

			if (ViewMode == ViewMode.CombatNPC)
			{
				int sortOrder = InfoCombat.sortedInfo.Values.Count;
				foreach (var type in InfoCombat.sortedInfo.Values)
				{
					var info = InfoCombat.combatNPC[type];
					info.slot.sortOrder = sortOrder--;
					gridNPC._items.Add(info.slot);
					gridNPC._innerList.Append(info.slot);
				}
			}
			else if (ViewMode == ViewMode.SpawnNPC)
			{
				foreach (var key in dicSpawnNPC.Keys)
				{
					var slot = new UISpawnNPCSlot(key, dicSpawnNPC[key]);
					gridNPC._items.Add(slot);
					gridNPC._innerList.Append(slot);
				}
			}
			else if (ViewMode == ViewMode.DropItem)
			{
				foreach (var key in dicDropItem.Keys)
				{
					var slot = new UIItemSlot(key, dicDropItem[key]);
					gridNPC._items.Add(slot);
					gridNPC._innerList.Append(slot);
				}
			}
			try
			{
				gridNPC.UpdateOrder();
				gridNPC._innerList.Recalculate();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Write(ex.Message);
			}

			string replaceCaption = "Count:0";
			switch (ViewMode)
			{
				case ViewMode.CombatNPC:
					replaceCaption = $"Count: {gridNPC._items.Count}";
					break;

				case ViewMode.SpawnNPC:
					replaceCaption = $"Count: {gridNPC._items.Count} ({Main.npc.Count(x => x.active)})";
					break;

				case ViewMode.DropItem:
					replaceCaption = $"Count: {gridNPC._items.Count} ({Main.item.Count(x => x.active)})";
					break;
			}

            panelMain.caption = caption.Replace("Count:0", replaceCaption);
        }

		public ViewMode ViewMode
		{
			get
			{
				return btnViewMode.GetValue<ViewMode>();
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			switch (ViewMode)
			{
				case ViewMode.CombatNPC:
					InfoCombat.Update();
					updateNeeded = InfoCombat.isUpdate;
					break;

				case ViewMode.SpawnNPC:
					var bFilterTown = btnSpawnNPCFilter.GetValue<SpawnNPCFilter>() == SpawnNPCFilter.HideTownNPC;
					var npcList = Main.npc.Where(x => x.active && (bFilterTown ? !x.townNPC : true)).GroupBy(x => x.netID);
					if (dicSpawnNPC.Count != npcList.Count() || dicSpawnNPC.Sum(x => x.Value) != npcList.Sum(x => x.Count()))
					{
						dicSpawnNPC.Clear();
						foreach (var npc in npcList)
							dicSpawnNPC.Add(npc.ToArray()[0].netID, npc.Count());

						updateNeeded = true;
					}
					break;

				case ViewMode.DropItem:
					var itemList = Main.item.Where(x => x.active).GroupBy(x => x.netID);
					if (dicDropItem.Count != itemList.Count() || dicDropItem.Sum(x=> x.Value) != itemList.Sum(x => x.Sum(y => y.stack)))
					{
						dicDropItem.Clear();
						foreach (var item in itemList)
							dicDropItem.Add(item.ToArray()[0].netID, item.Sum(x => x.stack));
						updateNeeded = true;
					}

					break;

			}
			if (ViewMode == ViewMode.CombatNPC)
			{
			}
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

	enum ViewMode
	{
		CombatNPC,
		SpawnNPC,
		DropItem,
	}
	enum SpawnNPCFilter
	{
		All,
		HideTownNPC,
	}
}
