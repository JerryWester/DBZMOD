using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DBZMOD.Buffs
{
	public class KaiokenBuffX3 : ModBuff
	{
        private int kaioDamageTimer;
        public override void SetDefaults()
		{
			DisplayName.SetDefault("Kaioken X3");
			Description.SetDefault("3x Damage, 3x Speed, Drains Life.");
			Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegenCount = 0;
            player.moveSpeed *= 3f;
<<<<<<< HEAD
            player.maxRunSpeed *= 3f;
=======
>>>>>>> 090fab9dd248e77d3537c6a57f88521a3c9e4299
            player.meleeDamage *= 3f;
            player.rangedDamage *= 3f;
            player.magicDamage *= 3f;
            player.minionDamage *= 3f;
            player.thrownDamage *= 3f;
            MyPlayer.ModPlayer(player).KiDamage *= 3f;
            Lighting.AddLight(player.Center, 5f, 0f, 0f);
            kaioDamageTimer++;
            if (kaioDamageTimer > 4 && player.statLife >= 0)
            {
                player.statLife -= 1;
                kaioDamageTimer = 0;
            }
            if (DBZMOD.instance.thoriumLoaded)
            {
                player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).symphonicDamage *= 3f;
                player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).radiantBoost *= 3f;
            }
            if (DBZMOD.instance.tremorLoaded)
            {
<<<<<<< HEAD
                player.GetModPlayer<Tremor.MPlayer>(ModLoader.GetMod("Tremor")).alchemicalDamage *= 3f;
=======
                player.GetModPlayer<Tremor.MPlayer>(ModLoader.GetMod("Tremor")).alchemicalDamage *= 20f;
>>>>>>> 090fab9dd248e77d3537c6a57f88521a3c9e4299
            }
        }
	}
}