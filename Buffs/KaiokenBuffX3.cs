using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DBZMOD.Buffs
{
    public class KaiokenBuffX3 : TransBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Kaioken x3");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            KaioLightValue = 7f;
            Description.SetDefault("2x Damage, 2x Speed, Drains Life.");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            DamageMulti = 2f;
            SpeedMulti = 1f;
            HealthDrainRate = 32;
            KiDrainBuffMulti = 1.5f;
            base.Update(player, ref buffIndex);
        }
    }
}

