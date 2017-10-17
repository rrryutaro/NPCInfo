using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Newtonsoft.Json;

namespace NPCInfo
{
	public class DropItem
	{
		public int id;
		public string mod;
		public string name;
		public int min;
		public int max;
		public DropItem()
		{
		}
		public DropItem(Item item)
		{
			id = item.netID;
			name = item.Name;
			if (item.modItem != null)
			{
				mod = item.modItem.mod.Name;
				name = item.modItem.Name;
			}
			min = max = 1;
		}
	}

	public class DropItemList
	{
		public int rate;
		public bool expert;
		public List<DropItem> items = new List<DropItem>();
	}

	public class NPCDropInfo
    {
        public int id;
        public string mod;
        public string name;
        public List<DropItemList> list = new List<DropItemList>();

        public NPCDropInfo()
        {
        }

        public NPCDropInfo(NPC npc)
        {
            id = npc.netID;
            name = npc.FullName;
            if (npc.modNPC != null)
            {
                mod = npc.modNPC.mod.Name;
            }
        }
    }

    public class NPCDropInfoUnifiedRandom : UnifiedRandom
    {
        public static int npcID = 0;
        public static int LastMaxValue = 0;

        public override int Next(int maxValue)
        {
            int result = base.Next(maxValue);

            if (0 < Main.npc[npcID].type)
            {
                LastMaxValue = maxValue;
                result = 0;
            }
            return result;
        }
    }

    public class NPCDropInfoGlobalItem : GlobalItem
    {
        public static event Action<Item> OnSetDefaults = null;

        public override void SetDefaults(Item item)
        {
            if (OnSetDefaults != null)
            {
                OnSetDefaults(item);
            }
        }
    }

    public static class NPCDropInfoUtils
    {
        public static void OutputDropInfo()
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(NPCInfo.pathNPCDropInfo, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.UTF8))
            {
				NPCInfoTool.listDropInfo = NPCDropInfoUtils.GetNPCDropInfoList();
				sw.Write(JsonConvert.SerializeObject(NPCInfoTool.listDropInfo));
            }
        }

        public static List<NPCDropInfo> GetNPCDropInfoList()
        {
            List<NPCDropInfo> result = new List<NPCDropInfo>();

			var rand = Main.rand;
			bool expertMode = Main.expertMode;
			Main.rand = new LootUnifiedRandom();
			Main.expertMode = false;

			for (int netID = -65; netID < NPCLoader.NPCCount; netID++)
			//int netID = 1;
			{
				NPC npc = new NPC();
				npc.SetDefaults(netID);
				for (int i = 0; i < 100; i++)
				{
					try
					{
						LootUnifiedRandom.NextLoop(i);
						npc.NPCLoot();
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine($"{netID}: {ex.Message}");
					}
				}
				var info = new NPCDropInfo(npc);
				foreach (var a in LootUnifiedRandom.list)
				{
					foreach (var b in a.list.Where(x => 0 < x.items.Count))
					{
						var list = new DropItemList();
						list.rate = b.maxValue;
						list.expert = Main.expertMode;
						foreach (var c in b.items)
						{
							if (!info.list.Any(x => x.items.Any(y => y.id == c.Value.netID)) && !list.items.Any(x => x.id == c.Value.netID))
								list.items.Add(new DropItem(c.Value));
						}
						if (0 < list.items.Count)
						{
							info.list.Add(list);
						}
					}
				}
				if (0 < info.list.Count)
				{
					result.Add(info);
				}
			}
			Main.rand = rand;
			Main.expertMode = expertMode;

			return result;
        }
    }
}
