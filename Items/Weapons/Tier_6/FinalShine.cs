﻿using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Weapons.Tier_6
{
	public class FinalShine : BaseBeamItem
    {
		public override void SetDefaults()
		{
			item.shoot = mod.ProjectileType("FinalShineCharge");
			item.shootSpeed = 0f;
			item.damage = 184;
			item.knockBack = 3f;
			item.useStyle = 5;
			item.UseSound = SoundID.Item12;
			item.useAnimation = 260;
			item.useTime = 260;
			item.width = 40;
			item.noUseGraphic = true;
			item.height = 40;
			item.autoReuse = false;
			item.value = 120000;
			item.rare = 9;
            item.channel = true;
            kiDrain = 250;
			weaponType = "Beam";
	    }
	    public override void SetStaticDefaults()
		{
		    Tooltip.SetDefault("Maximum Charges = 12\nHold Right Click to Charge\nHold Left Click to Fire");
            DisplayName.SetDefault("Final Shine");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
		    recipe.AddIngredient(null, "FinalFlash", 1);
		    recipe.AddIngredient(ItemID.FragmentVortex, 18);
            recipe.AddTile(null, "KaiTable");
            recipe.SetResult(this);
	        recipe.AddRecipe();
		}
    }
}
