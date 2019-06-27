﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ChlorophyteHeadpiece : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("16% Increased Ki Damage\n12% Increased Ki Crit Chance\nMaximum Ki increased by 500.");
            DisplayName.SetDefault("Chlorophyte Headpiece");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 16;
            item.value = 30000;
            item.rare = 7;
            item.defense = 11;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.ChlorophytePlateMail && legs.type == ItemID.ChlorophyteGreaves;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Getting hit gives greatly increased life regen and ki regen.";
            MyPlayer.ModPlayer(player).chlorophyteHeadPieceActive = true;
        }
        public override void UpdateEquip(Player player)
        {
            MyPlayer.ModPlayer(player).kiDamage += 0.16f;
            MyPlayer.ModPlayer(player).kiCrit += 12;
            MyPlayer.ModPlayer(player).kiMax2 += 500;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}