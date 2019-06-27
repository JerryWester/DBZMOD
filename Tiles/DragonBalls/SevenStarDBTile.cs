﻿﻿using Microsoft.Xna.Framework;
 using Terraria.ModLoader;

namespace DBZMOD.Tiles.DragonBalls
{
    public class SevenStarDBTile : DragonBallTile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("7 Star Dragon Ball");
            drop = mod.ItemType("SevenStarDB");
            AddMapEntry(new Color(249, 193, 49), name);
            disableSmartCursor = true;
            whichDragonBallAmI = 7;
        }
    }
}