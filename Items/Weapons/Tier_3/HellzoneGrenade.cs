﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Weapons.Tier_3
{
	public class HellzoneGrenade : KiItem
	{
		public override void SetDefaults()
		{
			item.shoot = mod.ProjectileType("HellzoneGrenadeProjectile");
			item.shootSpeed = 17f;
			item.damage = 52;
			item.knockBack = 6f;
            item.useStyle = 5;
			item.UseSound = SoundID.Item1;
			item.useAnimation = 28;
			item.useTime = 18;
			item.width = 40;
			item.noUseGraphic = true;
			item.height = 40;
			item.autoReuse = true;
			item.value = 9500;
			item.rare = 3;
            kiDrain = 75;
			weaponType = "Barrage";
			if(!Main.dedServ)
            {
                item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Scatterbeam").WithPitchVariance(.2f);
            }
	    }
	    public override void SetStaticDefaults()
		{
		DisplayName.SetDefault("Hellzone Grenade");
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(35));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PridefulKiCrystal", 30);
            recipe.AddIngredient(null, "SkeletalEssence", 20);
            recipe.AddTile(null, "ZTable");
            recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
