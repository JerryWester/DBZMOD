﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AdamantiteVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("12% Increased Ki Damage\n10% Increased Ki Crit Chance\nMaximum Ki increased by 250.");
            DisplayName.SetDefault("Adamantite Visor");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 16;
            item.value = 11000;
            item.rare = 4;
            item.defense = 9;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "7% Increased Ki Damage";
            MyPlayer.ModPlayer(player).kiDamage += 0.07f;
        }

        public override void UpdateEquip(Player player)
        {
            MyPlayer.ModPlayer(player).kiDamage += 0.12f;
            MyPlayer.ModPlayer(player).kiCrit += 10;
            MyPlayer.ModPlayer(player).kiMax2 += 250;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AdamantiteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}