﻿namespace DBZMOD.Items.Materials
{
    public class ScrapMetal : DBItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scrap Metal");
            Tooltip.SetDefault("'An old piece of metal, seems like something a junk merchant would sell.'");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.maxStack = 9999;
            item.value = 500;
            item.rare = 2;
        }
    }
}