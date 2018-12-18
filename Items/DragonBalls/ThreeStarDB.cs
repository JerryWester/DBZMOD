using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.DragonBalls
{
    public class ThreeStarDB : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("3 Star Dragon Ball");
            Tooltip.SetDefault("A mystical ball with 3 stars inscribed on it.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.value = 0;
            item.rare = -12;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = mod.TileType("ThreeStarDBTile");
        }
    }
}