﻿namespace DBZMOD.Items.Materials
{
    public class EarthenShard : DBItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earthen Shard");
            Tooltip.SetDefault("'A fragment of the land's soul.'");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 16;
            item.maxStack = 9999;
            item.value = 500;
            item.rare = 3;
        }
    }
}