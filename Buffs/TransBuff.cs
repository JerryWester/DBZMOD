﻿﻿using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using DBZMOD;
using Terraria;
using System;
using DBZMOD.Items.Accessories;
using Util;

namespace DBZMOD
{
    public abstract class TransBuff : ModBuff
    {
        public float DamageMulti;
        public float SpeedMulti;
        public float KaioLightValue;
        public float KiDrainBuffMulti;
        public float SSJLightValue;
        public int HealthDrainRate;
        public int OverallHealthDrainRate;
        public int KiDrainRate;
        private int KiDrainTimer;
        private int KiDrainAddTimer;
        public bool RealismModeOn;
        public int MasteryTimer;
        
        public override void Update(Player player, ref int buffIndex)
        {
            MyPlayer modPlayer = MyPlayer.ModPlayer(player);

            KiDrainAdd(player);
            if(Transformations.IsKaioken(player))
            {
                Lighting.AddLight(player.Center, KaioLightValue, 0f, 0f);
            }

            // only neuter the life regen if this is a draining buff.
            if (HealthDrainRate > 0)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;

                // only apply the kaio crystal benefit if this is kaioken
                bool isKaioCrystalEquipped = player.IsAccessoryEquipped("Kaio Crystal");
                float drainMult = (Transformations.IsKaioken(player) && isKaioCrystalEquipped ? 0.5f : 1f);

                // recalculate the final health drain rate and reduce regen by that amount
                OverallHealthDrainRate = (int)Math.Ceiling((float)HealthDrainRate * drainMult);
                player.lifeRegen -= OverallHealthDrainRate;
            }
            
            // if the player is in any ki-draining state, handles ki drain and power down when ki is depleted
            if (Transformations.IsSSJ(player) || Transformations.IsLSSJ(player) || Transformations.IsSSJ1Kaioken(player) || Transformations.IsAscended(player))
            {
                // player ran out of ki, so make sure they fall out of any forms they might be in.
                if (modPlayer.IsKiDepleted())
                {
                    // Main.NewText(string.Format("Player is out of ki??! {0} has {1} of {2} ki", player.whoAmI, modPlayer.GetKi(), modPlayer.OverallKiMax()));
                    Transformations.EndTransformations(player, true, false);
                }
                else
                {
                    // player still has some ki, perform drain routine
                    KiDrainTimer++;
                    if (KiDrainTimer > 2)
                    {
                        modPlayer.AddKi((KiDrainRate + modPlayer.KiDrainAddition) * -1);
                        KiDrainTimer = 0;
                    }
                    KiDrainAddTimer++;
                    if (KiDrainAddTimer > 600)
                    {
                        modPlayer.KiDrainAddition += 1;
                        KiDrainAddTimer = 0;
                    }
                    Lighting.AddLight(player.Center, 1f, 1f, 0f);
                }
            } else
            {
                // the player isn't in a ki draining state anymore, reset KiDrainAddition
                modPlayer.KiDrainAddition = 0;                
            }

            //DebugUtil.Log(string.Format("Before: Player moveSpeed {0} maxRunSpeed {1} runAcceleration {2} bonusSpeedMultiplier {3} speedMult {4}", player.moveSpeed, player.maxRunSpeed, player.runAcceleration, modPlayer.bonusSpeedMultiplier, SpeedMulti));
            player.moveSpeed *= 1f + (SpeedMulti * modPlayer.bonusSpeedMultiplier);
            player.maxRunSpeed *= 1f + (SpeedMulti * modPlayer.bonusSpeedMultiplier);
            player.runAcceleration *= 1f + (SpeedMulti * modPlayer.bonusSpeedMultiplier);
            //DebugUtil.Log(string.Format("After: Player moveSpeed {0} maxRunSpeed {1} runAcceleration {2} bonusSpeedMultiplier {3} speedMult {4}", player.moveSpeed, player.maxRunSpeed, player.runAcceleration, modPlayer.bonusSpeedMultiplier, SpeedMulti));

            // set player damage  mults
            player.meleeDamage += (DamageMulti - 1) * 0.5f;
            player.rangedDamage += (DamageMulti - 1) * 0.5f;
            player.magicDamage += (DamageMulti - 1) * 0.5f;
            player.minionDamage += (DamageMulti - 1) * 0.5f;
            player.thrownDamage += (DamageMulti - 1) * 0.5f;
            modPlayer.KiDamage += (DamageMulti - 1);

            // cross mod support stuff
            if (DBZMOD.instance.thoriumLoaded)
            {
                ThoriumEffects(player);
            }
            if (DBZMOD.instance.tremorLoaded)
            {
                TremorEffects(player);
            }
            if (DBZMOD.instance.enigmaLoaded)
            {
                EnigmaEffects(player);
            }
            if (DBZMOD.instance.battlerodsLoaded)
            {
                BattleRodEffects(player);
            }
            if (DBZMOD.instance.expandedSentriesLoaded)
            {
                ExpandedSentriesEffects(player);
            }
        }

        public void ThoriumEffects(Player player)
        {
            player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).symphonicDamage += (DamageMulti - 1) * 0.5f;
            player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).radiantBoost += (DamageMulti - 1) * 0.5f;
        }

        public void TremorEffects(Player player)
        {
            player.GetModPlayer<Tremor.MPlayer>(ModLoader.GetMod("Tremor")).alchemicalDamage += (DamageMulti - 1) * 0.5f;
        }

        public void EnigmaEffects(Player player)
        {
            player.GetModPlayer<Laugicality.LaugicalityPlayer>(ModLoader.GetMod("Laugicality")).mysticDamage += (DamageMulti - 1) * 0.5f;
        }

        public void BattleRodEffects(Player player)
        {
            player.GetModPlayer<UnuBattleRods.FishPlayer>(ModLoader.GetMod("UnuBattleRods")).bobberDamage += (DamageMulti - 1) * 0.5f;
        }

        public void ExpandedSentriesEffects(Player player)
        {
            player.GetModPlayer<ExpandedSentries.ESPlayer>(ModLoader.GetMod("ExpandedSentries")).sentryDamage += (DamageMulti - 1) * 0.5f;
        }

        private void KiDrainAdd(Player player)
        {
            MyPlayer.ModPlayer(player).KiDrainMulti = KiDrainBuffMulti;
        }
    }
}

