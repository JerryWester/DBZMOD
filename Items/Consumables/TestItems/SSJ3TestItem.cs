﻿using DBZMOD.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Consumables.TestItems
{
    public class SSJ3TestItem : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.consumable = false;
            item.maxStack = 1;
            item.UseSound = SoundID.Item3;
            item.useStyle = 2;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.value = 0;
            item.expert = true;
            item.potion = false;
            item.noUseGraphic = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SSJ3 Test Item");
            Tooltip.SetDefault("Manually activates the ssj3 transformation cutscene and unlocks it.");
        }


        public override bool UseItem(Player player)
        {
            MyPlayer.ModPlayer(player).SSJ3Transformation();
            UI.TransMenu.menuSelection = MenuSelectionID.SSJ3;
            MyPlayer.ModPlayer(player).ssj3Achieved = true;
            MyPlayer.ModPlayer(player).isTransforming = true;
            return true;

        }
    }
}
