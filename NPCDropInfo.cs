using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using Terraria.ModLoader;
using Newtonsoft.Json;

namespace NPCInfo
{
    public class NPCDropInfo
    {
        public int NPCType;
        public string ModName;
        public string NPCName;
        public class DropItem
        {
            public int ItemType;
            public string ModName;
            public string ItemName;
            public int Chance;
            public DropItem()
            {
            }
            public DropItem(Item item)
            {
                ItemType = item.type;
                ItemName = item.Name;
                if (item.modItem != null)
                {
                    ModName = item.modItem.mod.Name;
                    ItemName = item.modItem.Name;
                }
            }
        }
        public List<DropItem> Items = new List<DropItem>();

        public NPCDropInfo()
        {
        }

        public NPCDropInfo(NPC npc, List<Item> dropItems, List<Item> chanceItems)
        {
            NPCType = npc.type;
            NPCName = npc.FullName;
            if (npc.modNPC != null)
            {
                ModName = npc.modNPC.mod.Name;
            }
            foreach (var item in dropItems)
            {
                DropItem dropItem = new DropItem(item);
                var chanceItem = chanceItems.Where(x => x.type == dropItem.ItemType).ToArray();
                if (0 < chanceItem.Length)
                {
                    dropItem.Chance = chanceItem[0].value;
                }
                else
                {
                    dropItem.Chance = 0;
                }
                Items.Add(dropItem);
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
        private static List<Item> listChanceItems = new List<Item>();

        public static void OutputDropInfo()
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(NPCInfo.pathNPCDropInfo, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.UTF8))
            {
                sw.Write(JsonConvert.SerializeObject(NPCDropInfoUtils.GetNPCDropInfoList()));
            }
        }

        public static List<NPCDropInfo> GetNPCDropInfoList()
        {
            List<NPCDropInfo> result = new List<NPCDropInfo>();

            var rand = Main.rand;
            Main.rand = new NPCDropInfoUnifiedRandom();

            NPCDropInfoGlobalItem.OnSetDefaults += NPCDropInfoGlobalItem_OnSetDefaults;

            int npcID = 0;
            for (int type = 1; type < NPCLoader.NPCCount; type++)
            {
                try
                {
                    NPC npc = new NPC();
                    npc.SetDefaults(type);

                    listChanceItems.Clear();

                    NPCDropInfoUnifiedRandom.npcID = npcID;
                    npc.NPCLoot();
                    var list = Main.item.Where(x => 0 < x.type).ToList();
                    result.Add(new NPCDropInfo(npc, list, listChanceItems));

                    list.ForEach(x => x.SetDefaults(0));

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{type}: {ex.Message}");
                }
            }

            Main.rand = rand;
            NPCDropInfoGlobalItem.OnSetDefaults -= NPCDropInfoGlobalItem_OnSetDefaults;

            return result;
        }

        private static void NPCDropInfoGlobalItem_OnSetDefaults(Item obj)
        {
            obj.value = NPCDropInfoUnifiedRandom.LastMaxValue;
            listChanceItems.Add(obj);
        }
    }
}
