﻿using Terraria;
using Terraria.ModLoader;

namespace DBZMOD.Buffs
{
    public class KiLanternBuff : ModBuff
    {
        private int _lanternTimer;
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Ki Diffuser");
            Description.SetDefault("Gives some slight ki regen.");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MyPlayer.ModPlayer(player).AddKi(0.15f, false, false);
        }
    }
}
