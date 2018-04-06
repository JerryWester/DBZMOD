using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using DBZMOD;

namespace DBZMOD.Buffs
{
	public class KaiokenBuffX20 : ModBuff
	{
        private int kaioDamageTimer;
        public override void SetDefaults()
		{
			DisplayName.SetDefault("Kaioken x20");
			Description.SetDefault("20x Damage, 20x Speed, Rapidly Drains Life.");
			Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegenCount = 0;
            player.moveSpeed *= 20f;
<<<<<<< HEAD
            player.maxRunSpeed *= 20f;
=======
>>>>>>> 090fab9dd248e77d3537c6a57f88521a3c9e4299
            player.meleeDamage *= 20f;
            player.rangedDamage *= 20f;
            player.magicDamage *= 20f;
            player.minionDamage *= 20f;
            player.thrownDamage *= 20f;
            MyPlayer.ModPlayer(player).KiDamage *= 20f;
            Lighting.AddLight(player.Center, 5f, 0f, 0f);
            kaioDamageTimer++;
            if (kaioDamageTimer > 1 && player.statLife >= 0)
            {
                player.statLife -= 1;
                kaioDamageTimer = 0;
            }
            if (DBZMOD.instance.thoriumLoaded)
            {
                player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).symphonicDamage *= 20f;
                player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).radiantBoost *= 20f;
            }
            if (DBZMOD.instance.tremorLoaded)
            {
                player.GetModPlayer<Tremor.MPlayer>(ModLoader.GetMod("Tremor")).alchemicalDamage *= 20f;
            }
        }
	}
}