﻿﻿using System;
using DBZMOD.UI;
using Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Consumables.TestItems
{
    public class LSSJTestItem : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.consumable = true;
            item.maxStack = 1;
            item.UseSound = SoundID.Item3;
            item.useStyle = 2;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.value = 0;
            item.expert = true;
            item.potion = false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LSSJ Test Item");
            Tooltip.SetDefault("Manually activates the lssj transformation cutscene and unlocks it.");
        }


        public override bool UseItem(Player player)
        {
            MyPlayer.ModPlayer(player).LSSJTransformation();
            UI.TransMenu.MenuSelection = MenuSelectionID.LSSJ1;
            MyPlayer.ModPlayer(player).LSSJAchieved = true;
            MyPlayer.ModPlayer(player).IsTransforming = true;
            return true;

        }
    }
}
