using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using NPCInfo.UIElements;

namespace NPCInfo
{
    public class CombatNPCInfo
    {
        public DateTime timeStart;
        public DateTime timeUpdate;
        public UICombatNPCSlot slot;
        public CombatNPCInfo(NPC npc)
        {
            timeStart = DateTime.Now;
            timeUpdate = DateTime.Now;
            slot = new UICombatNPCSlot((NPC)npc.Clone());
        }
        public string GetSortedKey()
        {
            string result = $"{timeStart:ddHHmmssfff}_{slot.npc.netID.ToString("000").Replace("-", "")}";
            return result;
        }
    }

    public static class InfoCombat
    {
        public static Dictionary<int, CombatNPCInfo> combatNPC = new Dictionary<int, CombatNPCInfo>();
        public static SortedList<string, int> sortedInfo = new SortedList<string, int>();
        public static bool isUpdate;

        public static void Combat(NPC npc)
        {
            if (!combatNPC.ContainsKey(npc.netID))
            {
                combatNPC.Add(npc.netID, new CombatNPCInfo(npc));
                sortedInfo.Add(combatNPC[npc.netID].GetSortedKey(), npc.netID);
            }
            else
            {
                sortedInfo.Remove(combatNPC[npc.netID].GetSortedKey());
                combatNPC[npc.netID].timeUpdate = DateTime.Now;
                sortedInfo.Add(combatNPC[npc.netID].GetSortedKey(), npc.netID);
            }
            isUpdate = true;
        }
        public static void Update()
        {
            try
            {
                List<int> removeKeys = new List<int>();
                foreach (var key in combatNPC.Keys)
                {
                    var info = combatNPC[key];
                    TimeSpan ts = DateTime.Now - info.timeUpdate;
                    if (ts.TotalSeconds > Config.timeOut)
                    {
                        removeKeys.Add(key);
                    }
                }
                if (0 < removeKeys.Count)
                {
                    foreach (var key in removeKeys)
                    {
                        sortedInfo.Remove(combatNPC[key].GetSortedKey());
                        combatNPC.Remove(key);
                    }
                    isUpdate = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }

    public class InfoCombatPlayer : ModPlayer
    {
        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (victim is NPC && !(victim as NPC).townNPC)
            {
                InfoCombat.Combat(victim as NPC);
            }
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            InfoCombat.Combat(npc);
        }
    }
}
