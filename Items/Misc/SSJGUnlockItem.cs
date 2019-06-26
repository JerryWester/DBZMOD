﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DBZMOD.Util;

namespace DBZMOD.Items.Misc
{
    public class SSJGUnlockItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Godly Spirit");
            ItemID.Sets.ItemNoGravity[item.type] = true;
	        Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 3));
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 20;
        }

        public override bool ItemSpace(Player player)
        {
            return true;
        }

        public override bool CanPickup(Player player)
        {
            return true;
        }

        public override bool OnPickup(Player player)
        {
            SoundHelper.PlayVanillaSound(SoundID.NPCDeath7, player);
            TransformationHelper.EndTransformations(player);
            MyPlayer.ModPlayer(player).SSJGTransformation();
            MyPlayer.ModPlayer(player).isTransforming = true;
            MyPlayer.ModPlayer(player).ssjgAchieved = true;
            Main.NewText("You feel enveloped in a divine energy.");
            return false;
        }
			public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
 }