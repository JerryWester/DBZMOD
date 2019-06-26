﻿using System;
using DBZMOD.Buffs.SSJBuffs;
using DBZMOD.Util;
using Terraria;
using Terraria.ModLoader;

namespace DBZMOD.Buffs
{
    public abstract class TransBuff : ModBuff
    {
        public float damageMulti;
        public float speedMulti;
        public float kiDrainBuffMulti;
        public float ssjLightValue;
        public int healthDrainRate;
        public int overallHealthDrainRate;
        public float kiDrainRate;
        public float kiDrainRateWithMastery;
        private int _kiDrainAddTimer;
        public bool realismModeOn;
        public int baseDefenceBonus;
        public int precentDefenceBonus;

        public override void Update(Player player, ref int buffIndex)
        {
            MyPlayer modPlayer = MyPlayer.ModPlayer(player);
            
            KiDrainAdd(player);
            if(TransformationHelper.IsAnyKaioken(player) || TransformationHelper.IsSSJG(player))
            {
                Lighting.AddLight(player.Center + player.velocity * 8f, 0.2f, 0f, 0f);
            } else if (TransformationHelper.IsLSSJ1(player) || TransformationHelper.IsLSSJ2(player))
            {                
                Lighting.AddLight(player.Center + player.velocity * 8f, 0f, 0.2f, 0f);
            } else if (TransformationHelper.IsSSJ1(player) || TransformationHelper.IsSSJ2(player) || TransformationHelper.IsSSJ2(player) || TransformationHelper.IsAssj(player) || TransformationHelper.IsUssj(player))
            {
                Lighting.AddLight(player.Center + player.velocity * 8f, 0.2f, 0.2f, 0f);
            } else if (TransformationHelper.IsSpectrum(player))
            {
                var rainbow = Main.DiscoColor;
                Lighting.AddLight(player.Center + player.velocity * 8f, rainbow.R / 512f, rainbow.G / 512f, rainbow.B / 512f);
            }

            //give bonus base defense
            player.statDefense += baseDefenceBonus;
            
            // if the player is in any ki-draining state, handles ki drain and power down when ki is depleted
            if (TransformationHelper.IsAnythingOtherThanKaioken(player))
            {
                // player ran out of ki, so make sure they fall out of any forms they might be in.
                if (modPlayer.IsKiDepleted())
                {
                    if (TransformationHelper.IsSuperKaioken(player))
                    {
                        modPlayer.kaiokenLevel = 0;
                    }
                    TransformationHelper.EndTransformations(player);
                }
                else
                {
                    modPlayer.AddKi((kiDrainRate + modPlayer.kiDrainAddition) * -1, false, true);
                    _kiDrainAddTimer++;
                    if (_kiDrainAddTimer > 600)
                    {
                        modPlayer.kiDrainAddition += 1;
                        _kiDrainAddTimer = 0;
                    }
                    Lighting.AddLight(player.Center, 1f, 1f, 0f);
                }
            } else
            {
                // the player isn't in a ki draining state anymore, reset KiDrainAddition
                modPlayer.kiDrainAddition = 0;                
            }
            
            player.moveSpeed *= GetModifiedSpeedMultiplier(modPlayer);
            player.maxRunSpeed *= GetModifiedSpeedMultiplier(modPlayer);
            player.runAcceleration *= GetModifiedSpeedMultiplier(modPlayer);
            if (player.jumpSpeedBoost < 1f)
                player.jumpSpeedBoost = 1f;
            player.jumpSpeedBoost *= GetModifiedSpeedMultiplier(modPlayer);

            // set player damage  mults
            player.meleeDamage *= GetHalvedDamageBonus();
            player.rangedDamage *= GetHalvedDamageBonus();
            player.magicDamage *= GetHalvedDamageBonus();
            player.minionDamage *= GetHalvedDamageBonus();
            player.thrownDamage *= GetHalvedDamageBonus();
            modPlayer.KiDamage *= damageMulti;

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
            if (DBZMOD.instance.calamityLoaded)
            {
                CalamityEffects(player);
            }
        }

        public float GetModifiedSpeedMultiplier(MyPlayer modPlayer)
        {
            return 1f + ((speedMulti - 1f) * modPlayer.bonusSpeedMultiplier);
        }

        public float GetHalvedDamageBonus()
        {
            return 1f + ((damageMulti - 1f) * 0.5f);
        }

        public void ThoriumEffects(Player player)
        {
            player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).symphonicDamage *= GetHalvedDamageBonus();
            player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).radiantBoost *= GetHalvedDamageBonus();
        }

        public void TremorEffects(Player player)
        {
            player.GetModPlayer<Tremor.MPlayer>(ModLoader.GetMod("Tremor")).alchemicalDamage *= GetHalvedDamageBonus();
        }

        public void EnigmaEffects(Player player)
        {
            player.GetModPlayer<Laugicality.LaugicalityPlayer>(ModLoader.GetMod("Laugicality")).mysticDamage *= GetHalvedDamageBonus();
        }

        public void BattleRodEffects(Player player)
        {
            player.GetModPlayer<UnuBattleRods.FishPlayer>(ModLoader.GetMod("UnuBattleRods")).bobberDamage *= GetHalvedDamageBonus();
        }

        public void ExpandedSentriesEffects(Player player)
        {
            player.GetModPlayer<ExpandedSentries.ESPlayer>(ModLoader.GetMod("ExpandedSentries")).sentryDamage *= GetHalvedDamageBonus();
        }
        public void CalamityEffects(Player player)
        {
            //player.GetModPlayer<CalamityMod.CalamityCustomThrowingDamagePlayer>(ModLoader.GetMod("CalamityMod")).throwingDamage *= GetHalvedDamageBonus();
        }

        private void KiDrainAdd(Player player)
        {
            MyPlayer.ModPlayer(player).kiDrainMulti = kiDrainBuffMulti;
        }

        public string GetPercentForDisplay(string currentDisplayString, string text, int percent)
        {
            if (percent == 0)
                return currentDisplayString;
            return string.Format("{0}{1} {2}{3}%", currentDisplayString, text, percent > 0 ? "+" : string.Empty, percent);
        }

        public int kaiokenLevel = 0;
        public string AssembleTransBuffDescription()
        {
            string kaiokenName = string.Empty;
            if (Type == TransformationHelper.Kaioken.GetBuffId() || Type == TransformationHelper.SuperKaioken.GetBuffId())
            {
                switch (kaiokenLevel)
                {
                    case 2:
                        kaiokenName = "(x3)\n";
                        break;
                    case 3:
                        kaiokenName = "(x4)\n";
                        break;
                    case 4:
                        kaiokenName = "(x10)\n";
                        break;
                    case 5:
                        kaiokenName = "(x20)\n";
                        break;
                }
            }
            int percentDamageMult = (int)Math.Round(damageMulti * 100f, 0) - 100;
            int percentSpeedMult = (int)Math.Round(speedMulti * 100f, 0) - 100;
            float kiDrainPerSecond = 60f * kiDrainRate;
            float kiDrainPerSecondWithMastery = 60f * kiDrainRateWithMastery;
            int percentKiDrainMulti = (int)Math.Round(kiDrainBuffMulti * 100f, 0) - 100;
            string displayString = kaiokenName;
            displayString = GetPercentForDisplay(displayString, "Damage", percentDamageMult);
            displayString = GetPercentForDisplay(displayString, " Speed", percentSpeedMult);
            displayString = GetPercentForDisplay(displayString, "\nKi Costs", percentKiDrainMulti);
            if (kiDrainPerSecond > 0)
            {
                displayString = string.Format("{0}\nKi Drain: {1}/s", displayString, (int)Math.Round(kiDrainPerSecond, 0));
                if (kiDrainPerSecondWithMastery > 0)
                {
                    displayString = string.Format("{0}, {1}/s when mastered", displayString, (int)Math.Round(kiDrainPerSecondWithMastery, 0));
                }
            }
            if (healthDrainRate > 0)
            {
                displayString = string.Format("{0}\nLife Drain: -{1}/s.", displayString, healthDrainRate / 2);
            }
            return displayString;
        }

        public static int GetTotalHealthDrain(Player player)
        {
            var healthDrain = KaiokenBuff.GetHealthDrain(player.GetModPlayer<MyPlayer>()) + SuperKaiokenBuff.GetHealthDrain(player);
            return healthDrain;
        }
    }
}

