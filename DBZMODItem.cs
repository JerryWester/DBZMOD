﻿﻿using Terraria.ID;
 using System.Collections.Generic;
 using Terraria;
using Terraria.ModLoader;

namespace DBZMOD
{
    public class DBZMODItem : GlobalItem
    {
        public int kiChangeBonus;
        public int speedChangeBonus;
        public int maxChargeBonus;
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public DBZMODItem()
        {
            kiChangeBonus = 0;
            speedChangeBonus = 0;
            maxChargeBonus = 0;
        }
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            DBZMODItem dbzClone = (DBZMODItem)base.Clone(item, itemClone);
            dbzClone.kiChangeBonus = kiChangeBonus;
            dbzClone.speedChangeBonus = speedChangeBonus;
            dbzClone.maxChargeBonus = maxChargeBonus;
            return dbzClone;
        }
        
        //Broken right now, smh
        /*public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if ((item.modItem is KiItem && item.damage > 0) && item.maxStack == 1 && rand.NextBool(10))
            {
                return mod.PrefixType("CondensedPrefix");
            }
            if ((item.modItem is KiItem && item.damage > 0) && item.maxStack == 1 && rand.NextBool(60))
            {
                return mod.PrefixType("MystifyingPrefix");
            }
            if ((item.modItem is KiItem && item.damage > 0) && item.maxStack == 1 && rand.NextBool(30))
            {
                return mod.PrefixType("UnstablePrefix");
            }
            if ((item.modItem is KiItem && item.damage > 0) && item.maxStack == 1 && rand.NextBool(10))
            {
                return mod.PrefixType("BalancedPrefix");
            }
            if ((item.modItem is KiItem && item.damage > 0) && item.maxStack == 1 && rand.NextBool(5))
            {
                return mod.PrefixType("MasteredPrefix");
            }
            return -1;
        }*/
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!item.social && item.prefix > 0)
			{
                // try to find the end of the prefix indexes. If one can't be found, just slap it at the end?
                int index = tooltips.FindLastIndex(t => (t.mod == "Terraria" || t.mod == mod.Name) && (t.isModifier || t.Name.StartsWith("Tooltip")));
                if (index == -1)
                {
                    index = tooltips.Count - 1;
                }
				if (kiChangeBonus > 0)
				{
					TooltipLine line = new TooltipLine(mod, "PrefixKiChange", "+" + kiChangeBonus + "% More Ki Usage");
					line.isModifier = true;
                    line.isModifierBad = true;
					tooltips.Insert(index, line);
				}
                if (kiChangeBonus < 0)
				{
					TooltipLine line = new TooltipLine(mod, "PrefixKiChange", kiChangeBonus + "% Less Ki Usage");
					line.isModifier = true;
					tooltips.Insert(index, line);
				}
                if (speedChangeBonus > 0)
				{
					TooltipLine line = new TooltipLine(mod, "PrefixSpeedChange", "+" + speedChangeBonus + "% More cast speed");
					line.isModifier = true;
					tooltips.Insert(index, line);
				}
                if (speedChangeBonus < 0)
				{
					TooltipLine line = new TooltipLine(mod, "PrefixSpeedChange", speedChangeBonus + "% Less cast speed");
					line.isModifier = true;
                    line.isModifierBad = true;
					tooltips.Insert(index, line);
				}
                if (maxChargeBonus > 0)
				{
					TooltipLine line = new TooltipLine(mod, "PrefixKiChange", "+" + maxChargeBonus + " Maximum charges");
					line.isModifier = true;
					tooltips.Insert(index, line);
				}
            }
        } 
           
        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.EyeOfCthulhuBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KaioFragmentFirst"));
                }
            }
            if(Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && (arg == ItemID.EaterOfWorldsBossBag || arg == ItemID.BrainOfCthulhuBossBag))
                {
                    player.QuickSpawnItem(mod.ItemType("KaioFragment1"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.QueenBeeBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KaioFragment2"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.SkeletronBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KaioFragment3"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.WallOfFleshBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KaioFragment4"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.EyeOfCthulhuBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KiFragment1"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.SkeletronBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KiFragment2"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.SkeletronPrimeBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KiFragment3"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.PlanteraBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KiFragment4"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.CultistBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("KiFragment5"));
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                if (context == "bossBag" && arg == ItemID.WallOfFleshBossBag)
                {
                    player.QuickSpawnItem(mod.ItemType("SpiritualEmblem"));
                }
            }
            if (context == "bossBag" && arg == ItemID.WallOfFleshBossBag)
            {
                player.QuickSpawnItem(mod.ItemType("TransmissionVanish"));
            }
            if (context == "bossBag" && arg == ItemID.FishronBossBag)
            {
                player.QuickSpawnItem(mod.ItemType("KatchinScale"), Main.rand.Next(18, 42));
            }
        }
    }
}