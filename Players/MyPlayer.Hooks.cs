﻿using System;
using System.Collections.Generic;
using System.Linq;
using DBZMOD.Buffs;
using DBZMOD.Config;
using DBZMOD.Dynamicity;
using DBZMOD.Effects.Animations.Aura;
using DBZMOD.Enums;
using DBZMOD.Extensions;
using DBZMOD.Network;
using DBZMOD.Traits;
using DBZMOD.Transformations;
using DBZMOD.UI;
using DBZMOD.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD
{
    public partial class MyPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
            
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetworkHelper.playerSync.RequestServerSendKiBeaconInitialSync(256, Main.myPlayer);
                NetworkHelper.playerSync.RequestAllDragonBallLocations(256, Main.myPlayer);
            }
        }

        public override void PlayerConnect(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (player.whoAmI != Main.myPlayer)
                {
                    NetworkHelper.playerSync.SendPlayerInfoToPlayerFromOtherPlayer(player.whoAmI, Main.myPlayer);

                    NetworkHelper.playerSync.RequestPlayerSendTheirInfo(256, Main.myPlayer, player.whoAmI);
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (player.IsSSJG())
            {
                drawInfo.hairColor = new Color(255, 57, 74);
                drawInfo.hairShader = 1;
                ChangeEyeColor(Color.Red);
            }
            else if (player.IsSSJ() || player.IsLSSJ() || player.IsAssj() || player.IsUssj())
            {
                ChangeEyeColor(Color.Turquoise);
            }
            else if (player.IsAnyKaioken())
            {
                ChangeEyeColor(Color.Red);
            }
            else if (originalEyeColor.HasValue && player.eyeColor != originalEyeColor.Value)
            {
                player.eyeColor = originalEyeColor.Value;
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (radiantBonus && _kiCurrent < OverallKiMax())
            {
                int i = Main.rand.Next(1, 6);
                AddKi(i, false, false);
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(51, 204, 255), i, false, false);
                if (Main.rand.Next(2) == 0)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0, 20, mod.ProjectileType("RadiantSpark"), (int)kiDamage * 100, 0, player.whoAmI);
                }
            }
            if (metamoranSash)
            {
                if (Main.rand.NextBool(15))
                {
                    damage *= 2;
                }
            }
            if (metamoranSash)
            {
                if (Main.rand.NextBool(5))
                {
                    damage *= 2;
                }
            }
            base.OnHitNPC(item, target, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (radiantBonus && _kiCurrent < OverallKiMax())
            {
                int i = Main.rand.Next(1, 6);
                AddKi(i, false, false);
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(51, 204, 255), i, false, false);
                if (Main.rand.Next(3) == 0)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0, 20, mod.ProjectileType("RadiantSpark"), (int)kiDamage * 100, 0, player.whoAmI);
                }
            }
            base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
        }

        public override void UpdateBiomeVisuals()
        {
            //bool useGodSky = Transformations.IsGodlike(player);
            //player.ManageSpecialBiomeVisuals("DBZMOD:GodSky", useGodSky, player.Center);
            player.ManageSpecialBiomeVisuals("DBZMOD:WishSky", wishActive, player.Center);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            UpdateSynchronizedControls(triggersSet);

            SyncTriggerSet();

            if (flyToggle.JustPressed)
            {
                if (flightUnlocked)
                {
                    isFlying = !isFlying;
                    if (!isFlying)
                    {
                        FlightSystem.AddKatchinFeetBuff(player);
                    }
                }
            }

            _mProgressionSystem.Update(player);

            // dropping the fist wireup here. Fingers crossed.
            if (player.HeldItem.Name.Equals("Fist"))
            {
                _mFistSystem.Update(triggersSet, player, mod);
            }

            if (armorBonus.JustPressed)
            {
                if (demonBonus && !player.HasBuff(mod.BuffType("ArmorCooldown")))
                {
                    player.AddBuff(mod.BuffType("DemonBonus"), 300);
                    demonBonusActive = true;
                    for (int i = 0; i < 3; i++)
                    {
                        Dust tDust = Dust.NewDustDirect(player.position - (Vector2.UnitY * 0.7f) - (Vector2.UnitX * 1.0f), 50, 50, 15, 0f, 0f, 5, default(Color), 2.0f);
                    }
                }
            }

            // handle ki charging
            if (ConfigModel.isChargeToggled)
            {
                if (energyCharge.JustPressed)
                {
                    isCharging = !isCharging;
                }
            }
            else
            {
                if (energyCharge.Current && !isCharging)
                {
                    isCharging = true;
                }
                if (!energyCharge.Current && isCharging)
                {
                    isCharging = false;
                }
            }

            // most of the forms have a default light value, but charging isn't a buff. Let there be light
            if (isCharging && !player.IsKaioken() && !player.IsAnythingOtherThanKaioken())
            {
                Lighting.AddLight(player.Center, 1.2f, 1.2f, 1.2f);
            }

            // calls to handle transformation or kaioken powerups per frame

            HandleTransformations();

            HandleKaioken();

            if (speedToggle.JustPressed)
            {
                float oldSpeedMult = bonusSpeedMultiplier;
                bonusSpeedMultiplier = GetNextSpeedMultiplier();
                CombatText.NewText(player.Hitbox, new Color(255, 255, 255), string.Format("Speed bonus {0}!", (bonusSpeedMultiplier == 0f) ? "off" : ((int)Math.Ceiling(bonusSpeedMultiplier * 100f)).ToString() + "%"), false, false);
            }

            if (transMenu.JustPressed)
            {
                UI.TransformationMenu.menuVisible = !UI.TransformationMenu.menuVisible;
            }

            /*if (ProgressionMenuKey.JustPressed)
            {
                ProgressionMenu.ToggleVisibility();
            }*/

            // power down handling
            if (IsCompletelyPoweringDown() && player.IsPlayerTransformed())
            {
                var playerWasSuperKaioken = player.IsSuperKaioken();
                player.EndTransformations();
                if (playerWasSuperKaioken)
                {
                    player.DoTransform(DBZMOD.Instance.TransformationDefinitionManager.SSJ1Definition, mod);
                }
                kaiokenLevel = 0;
                SoundHelper.PlayCustomSound("Sounds/PowerDown", player, .3f);
            }


            if (wishActive)
            {
                WishMenu.menuVisible = true;
            }

            if (quickKi.JustPressed)
            {
                player.FindAndConsumeKiPotion();
            }

            // freeform instant transmission requires book 2.
            if (isInstantTransmission2Unlocked)
            {
                HandleInstantTransmissionFreeform();
            }
        }

        // occurs in vanilla code *just* before regen effects are applied.
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();

            // pretend the player has the shiny stone here - this is just in time for the vanilla regen calls to kick in.
            if (buldariumSigmite)
                player.shinyStone = true;
        }

        public override void ResetEffects()
        {
            kiDamage = 1f;
            kiKbAddition = 0f;
            kiChargeRate = 1;

            if (kiEssence1)
                kiChargeRate += 1;
            if (kiEssence2)
                kiChargeRate += 1;
            if (kiEssence3)
                kiChargeRate += 2;
            if (kiEssence4)
                kiChargeRate += 2;
            if (kiEssence5)
                kiChargeRate += 3;

            scouterT2 = false;
            scouterT3 = false;
            scouterT4 = false;
            scouterT5 = false;
            scouterT6 = false;
            spiritualEmblem = false;
            turtleShell = false;
            kiDrainMulti = 1f;
            kiSpeedAddition = 1f;
            kiCrit = 5;
            chlorophyteHeadPieceActive = false;
            diamondNecklace = false;
            emeraldNecklace = false;
            rubyNecklace = false;
            earthenSigil = false;
            radiantTotem = false;
            zenkaiCharm = false;
            majinNucleus = false;
            baldurEssentia = false;
            earthenArcanium = false;
            legendNecklace = false;
            legendWaistcape = false;
            kiChip = false;
            radiantGlider = false;
            dragongemNecklace = false;
            sapphireNecklace = false;
            topazNecklace = false;
            amberNecklace = false;
            amethystNecklace = false;
            kiOrbDropChance = 3;
            isHoldingKiWeapon = false;
            wornGloves = false;
            senzuBag = false;
            palladiumBonus = false;
            adamantiteBonus = false;
            orbGrabRange = 2;
            orbHealAmount = 50;
            demonBonus = false;
            blackFusionBonus = false;
            chargeLimitAdd = 0;
            flightSpeedAdd = 0;
            flightKiConsumptionMultiplier = 1f;
            kiRegen = 0;
            earthenScarab = false;
            hermitBonus = false;
            zenkaiCharmActive = false;
            chargeTimerMaxAdd = 0;
            spiritCharm = false;
            battleKit = false;
            armCannon = false;
            radiantBonus = false;
            crystalliteControl = false;
            crystalliteFlow = false;
            crystalliteAlleviate = false;
            chargeMoveSpeed = 0f;
            kaiokenDrainMulti = 1f;
            kaioCrystal = false;
            luminousSectum = false;
            infuserAmber = false;
            infuserAmethyst = false;
            infuserDiamond = false;
            infuserEmerald = false;
            infuserRainbow = false;
            infuserRuby = false;
            infuserSapphire = false;
            infuserTopaz = false;
            blackDiamondShell = false;
            buldariumSigmite = false;
            attunementBracers = false;
            burningEnergyAmulet = false;
            iceTalisman = false;
            pureEnergyCirclet = false;
            timeRing = false;
            bloodstainedBandana = false;
            goblinKiEnhancer = false;
            mechanicalAmplifier = false;
            metamoranSash = false;
            kiMax2 = 0;

            kiMaxMult = PlayerTrait == DBZMOD.Instance.TraitManager.Legendary ? 2f : 1f;

            isHoldingDragonRadarMk1 = false;
            isHoldingDragonRadarMk2 = false;
            isHoldingDragonRadarMk3 = false;
            eliteSaiyanBonus = false;
            healMulti = 1f;
        }

        // TODO Change this in favor of auto checking.
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {

            bool isAnyBossAlive = false;
            bool isGolemAlive = false;
            foreach (NPC npc in Main.npc)
            {
                if (npc.boss && npc.active)
                {
                    isAnyBossAlive = true;
                }
                if (npc.type == NPCID.Golem)
                {
                    isGolemAlive = true;
                }
            }
            if (zenkaiCharm && !zenkaiCharmActive && !player.HasBuff(mod.BuffType("ZenkaiCooldown")))
            {
                player.statLife = 50;
                player.HealEffect(50);
                player.AddBuff(mod.BuffType("ZenkaiBuff"), 300);
                return false;
            }
            if (eliteSaiyanBonus && !zenkaiCharmActive && !player.HasBuff(mod.BuffType("ZenkaiCooldown")))
            {
                int healamount = (player.statLifeMax + player.statLifeMax2);
                player.statLife += healamount;
                player.HealEffect(healamount);
                player.AddBuff(mod.BuffType("ZenkaiBuff"), 600);
                return false;
            }

            if (isAnyBossAlive && !SSJ1Achived && player.whoAmI == Main.myPlayer && NPC.downedBoss3)
            {
                if (rageCurrent >= 3)
                {
                    overallFormUnlockChance = 1;
                }
                else
                {
                    formUnlockChance = 20;
                }
                if ((Main.rand.Next(overallFormUnlockChance) == 0))
                {
                    Main.NewText("The humiliation of failing drives you mad.", Color.Yellow);
                    player.statLife = player.statLifeMax2 / 2;
                    player.HealEffect(player.statLifeMax2 / 2);

                    DBZMOD.Instance.TransformationDefinitionManager.LSSJDefinition.Unlock(this);

                    isTransforming = true;
                    SSJTransformation();
                    UI.TransformationMenu.SelectedTransformation = DBZMOD.Instance.TransformationDefinitionManager.SSJ1Definition;
                    rageCurrent = 0;
                    player.EndTransformations();
                    return false;
                }
            }

            if (isAnyBossAlive && SSJ1Achived && !SSJ2Achieved && player.whoAmI == Main.myPlayer && !IsLegendary() && NPC.downedMechBossAny && (player.IsSSJ1() || player.IsAssj() || player.IsUssj()) && masteryLevels[DBZMOD.Instance.TransformationDefinitionManager.SSJ1Definition.UnlocalizedName] >= 1)
            {
                Main.NewText("The rage of failing once more dwells deep within you.", Color.Red);
                player.statLife = player.statLifeMax2 / 2;
                player.HealEffect(player.statLifeMax2 / 2);

                DBZMOD.Instance.TransformationDefinitionManager.SSJ2Definition.Unlock(this);

                isTransforming = true;
                SSJ2Transformation();
                UI.TransformationMenu.SelectedTransformation = DBZMOD.Instance.TransformationDefinitionManager.SSJ2Definition;
                player.EndTransformations();
                rageCurrent = 0;
                return false;
            }

            if (isAnyBossAlive && SSJ1Achived && !LSSJAchieved && player.whoAmI == Main.myPlayer && IsLegendary() && NPC.downedMechBossAny && player.HasBuff(DBZMOD.Instance.TransformationDefinitionManager.SSJ1Definition.GetBuffId()) && masteryLevels[DBZMOD.Instance.TransformationDefinitionManager.SSJ1Definition.UnlocalizedName] >= 1)
            {
                Main.NewText("Your rage is overflowing, you feel something rise up from deep inside.", Color.Green);
                player.statLife = player.statLifeMax2 / 2;
                player.HealEffect(player.statLifeMax2 / 2);

                DBZMOD.Instance.TransformationDefinitionManager.LSSJDefinition.Unlock(this);

                isTransforming = true;
                LSSJTransformation();
                UI.TransformationMenu.SelectedTransformation = DBZMOD.Instance.TransformationDefinitionManager.LSSJDefinition;
                player.EndTransformations();
                rageCurrent = 0;
                return false;
            }

            if (isGolemAlive && SSJ1Achived && SSJ2Achieved && !SSJ3Achieved && !IsLegendary() && player.whoAmI == Main.myPlayer && player.HasBuff(DBZMOD.Instance.TransformationDefinitionManager.SSJ2Definition.GetBuffId()) && masteryLevels[DBZMOD.Instance.TransformationDefinitionManager.SSJ2Definition.UnlocalizedName] >= 1)
            {
                Main.NewText("The ancient power of the Lihzahrds seeps into you, causing your power to become unstable.", Color.Orange);
                player.statLife = player.statLifeMax2 / 2;
                player.HealEffect(player.statLifeMax2 / 2);

                DBZMOD.Instance.TransformationDefinitionManager.SSJ3Definition.Unlock(this);

                isTransforming = true;
                SSJ3Transformation();
                UI.TransformationMenu.SelectedTransformation = DBZMOD.Instance.TransformationDefinitionManager.SSJ3Definition;
                player.EndTransformations();
                rageCurrent = 0;
                return false;
            }

            if (immortalityRevivesLeft > 0)
            {
                int healamount = (player.statLifeMax + player.statLifeMax2);
                player.statLife += healamount;
                player.HealEffect(healamount);
                immortalityRevivesLeft -= 1;
                return false;
            }

            if (isAnyBossAlive && player.whoAmI == Main.myPlayer)
            {
                rageCurrent += 1;
                return true;
            }

            return true;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            // i frames during transformation
            if (isTransforming)
            {
                return false;
            }

            // PLACE DAMAGE CANCELLING EFFECTS HERE.

            // handle blocking damage reductions
            switch (blockState)
            {
                case 1:
                    return false; // damage negated
                case 2:
                    damage /= 3;
                    break;
                case 3:
                    damage /= 2;
                    break;
            }

            // handle chlorophyte regen
            if (chlorophyteHeadPieceActive && !player.HasBuff(mod.BuffType("ChlorophyteRegen")))
            {
                player.AddBuff(mod.BuffType("ChlorophyteRegen"), 180);
            }

            // handle ki enhancer "reserves" buff
            if (goblinKiEnhancer && !player.HasBuff(mod.BuffType("EnhancedReserves")))
            {
                player.AddBuff(mod.BuffType("EnhancedReserves"), 180);
            }

            // black diamond ki bonus
            if (blackDiamondShell)
            {
                int i = Main.rand.Next(10, 100);
                AddKi(i, false, false);
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(51, 204, 255), i, false, false);
            }

            // increment current mastery if applicable, for damage taken.
            HandleDamageReceivedMastery(damage);

            return true;
        }

        public override void SetupStartInventory(IList<Item> items)
        {
            Item item8 = new Item();
            item8.SetDefaults(mod.ItemType("EmptyNecklace"));
            item8.stack = 1;
            items.Add(item8);
        }

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            int hair = layers.FindIndex(l => l == PlayerHeadLayer.Hair);
            if (hair < 0)
                return;


            if (this.hair != null)
            {
                layers[hair] = new PlayerHeadLayer(mod.Name, "TransHair",
                    delegate (PlayerHeadDrawInfo draw)
                    {
                        Player player = draw.drawPlayer;

                        if (GetBaseStyle() == 0)
                            return;

                        Color alpha = draw.drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)(draw.drawOrigin.X + draw.drawPlayer.width * 0.5) / 16, (int)((draw.drawOrigin.Y + draw.drawPlayer.height * 0.25) / 16.0), hairColor), draw.drawPlayer.shadow);
                        DrawData data = new DrawData(this.hair, new Vector2((float)((int)(draw.drawOrigin.X - Main.screenPosition.X - (float)(player.bodyFrame.Width / 2) + (float)(player.width / 2))), (float)((int)(draw.drawOrigin.Y - Main.screenPosition.Y + (float)player.height - (float)player.bodyFrame.Height + 3.5f))) + player.headPosition + draw.drawOrigin, player.bodyFrame, alpha, player.headRotation, draw.drawOrigin, 1f, draw.spriteEffects, 0);
                        data.shader = draw.hairShader;
                        Main.playerDrawData.Add(data);
                    });
            }

            if (this.hair != null)
            {
                PlayerLayer.Head.visible = false;
                PlayerLayer.Hair.visible = false;
                PlayerLayer.HairBack.visible = false;
                PlayerHeadLayer.Hair.visible = false;
                PlayerHeadLayer.Head.visible = false;
            }
        }

        private Color hairColor;
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            //handle lightning effects
            AnimationHelper.lightningEffects.visible = true;
            layers.Add(AnimationHelper.lightningEffects);

            // handle transformation animations
            AnimationHelper.transformationEffects.visible = true;
            layers.Add(AnimationHelper.transformationEffects);

            // handle dragon radar drawing
            if (isHoldingDragonRadarMk1 || isHoldingDragonRadarMk2 || isHoldingDragonRadarMk3)
            {
                AnimationHelper.dragonRadarEffects.visible = true;
                layers.Add(AnimationHelper.dragonRadarEffects);
            }

            if (isPlayerUsingKiWeapon)
            {
                AnimationHelper.kiChargeAttackEffects.visible = true;
                layers.Add(AnimationHelper.kiChargeAttackEffects);
            }

            AnimationHelper.auraEffect.visible = true;
            // capture the back layer index, which should always exist before the hook fires.
            var index = layers.FindIndex(x => x.Name == "MiscEffectsBack");
            layers.Insert(index, AnimationHelper.auraEffect);

            // handle SSJ hair/etc.
            
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (victim != player && victim.whoAmI != NPCID.TargetDummy)
            {
                float expierenceToAdd = 10.0f;
                float experienceMult = 1.0f;

                if (player.IsPlayerTransformed())
                {
                    experienceMult = 2.0f;
                }

                _mProgressionSystem.AddKiExperience(expierenceToAdd * experienceMult);
            }
            base.OnHitAnything(x, y, victim);
        }

        public override void UpdateLifeRegen()
        {
            base.UpdateLifeRegen();
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            base.NaturalLifeRegen(ref regen);

            HandlePowerWishPlayerHealth();
            HandleOverloadHealthChange();
        }

        public override void UpdateBadLifeRegen()
        {
            base.UpdateBadLifeRegen();

            // Kaioken neuters regen and drains the player
            if (player.IsAnyKaioken())
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }

                player.lifeRegenTime = 0;

                // only apply the kaio crystal benefit if this is kaioken
                bool isKaioCrystalEquipped = player.IsAccessoryEquipped("Kaio Crystal");
                float drainMult = isKaioCrystalEquipped ? 0.5f : 1f;

                // recalculate the final health drain rate and reduce regen by that amount
                var healthDrain = (int)Math.Ceiling(TransformationBuff.GetTotalHealthDrain(player) * drainMult);
                player.lifeRegen -= healthDrain;
            }
        }

        public override void PostUpdateEquips()
        {
            HandlePowerWishMultipliers();
        }

        public void HandlePowerWishPlayerHealth()
        {
            player.statLifeMax2 = player.statLifeMax2 + GetPowerWishesUsed() * 20;
        }

        private float overloadHealthChange = 1f;
        public void HandleOverloadHealthChange()
        {
            if (IsOverloading())
            {
                overloadHealthChange = Main.rand.NextFloat(0.3f, 1.5f);
                player.statLifeMax2 = (int)Math.Ceiling(player.statLifeMax2 * overloadHealthChange);
            }
        }

        public override void PreUpdate()
        {
            if (player.IsPlayerTransformed())
            {
                if (!player.armor[10].vanity && player.armor[10].headSlot == -1)
                {
                    if (player.IsSSJ1())
                    {
                        hair = mod.GetTexture("Hairs/SSJ/SSJHair" + GetSSJ1Style());
                    }
                    else if (player.IsAssj())
                    {
                        hair = mod.GetTexture("Hairs/SSJ/ASSJHair" + GetASSJStyle());
                    }
                    else if (player.IsUssj())
                    {
                        hair = mod.GetTexture("Hairs/SSJ/USSJHair" + GetASSJStyle());
                    }
                    else if (player.IsSuperKaioken())
                    {
                        hair = mod.GetTexture("Hairs/SSJ/SSJ1KaiokenHair" + GetSSJKKStyle());
                    }
                    else if (player.IsSSJ2())
                    {
                        hair = mod.GetTexture("Hairs/SSJ2/SSJ2Hair" + GetSSJ2Style());
                    }
                    else if (player.IsSSJ3())
                    {
                        hair = mod.GetTexture("Hairs/SSJ3/SSJ3Hair" + GetSSJ3Style());
                    }
                    else if (player.IsLSSJ1())
                    {
                        hair = mod.GetTexture("Hairs/LSSJ/LSSJHair" + GetLSSJStyle());
                    }
                    else if (player.IsSpectrum())
                    {
                        hair = mod.GetTexture("Hairs/Dev/SSJSHair");
                    }
                    else if (!player.IsPlayerTransformed())
                    {
                        hair = mod.GetTexture("Hairs/Base/BaseHair" + GetBaseStyle());
                    }
                    if (player.IsSSJG())
                    {
                        hair = mod.GetTexture("Hairs/Base/BaseHair" + GetBaseStyle());
                    }
                }
            }
            else
            {
                hair = mod.GetTexture("Hairs/Base/BaseHair" + GetBaseStyle());
            }
            if (player.dead)
            {
                hair = mod.GetTexture("Hairs/Base/BaseHair" + GetBaseStyle());
            }
            if(!player.IsPlayerTransformed() || player.dead)
            {
                hairColor = player.hairColor;
            }
            else
            {
                hairColor = Color.White;
            }
        }
        #region Hair Style Checks
        public int GetBaseStyle()
        {
            return baseHairStyle;
        }
        public int GetSSJ1Style()
        {
            return ssj1HairStyle;
        }
        public int GetSSJ2Style()
        {
            return ssj2HairStyle;
        }
        public int GetSSJ3Style()
        {
            return ssj3HairStyle;
        }
        public int GetSSJ4Style()
        {
            return ssj4HairStyle;
        }
        public int GetSSJGStyle()
        {
            return baseHairStyle;
        }
        public int GetSSJBStyle()
        {
            return ssj1HairStyle;
        }
        public int GetSSJRStyle()
        {
            return ssj1HairStyle;
        }
        public int GetASSJStyle()
        {
            return ssj1HairStyle;
        }
        public int GetSSJCStyle()
        {
            return ssj1HairStyle;
        }
        public int GetLSSJStyle()
        {
            return ssj2HairStyle;
        }
        public int GetSSJBEStyle()
        {
            return ssj2HairStyle;
        }
        public int GetSSJKKStyle()
        {
            return ssj1HairStyle;
        }
        public int GetSSJBKKStyle()
        {
            return ssj1HairStyle;
        }
        public int GetSSJRKKStyle()
        {
            return ssj1HairStyle;
        }

        #endregion

        public override void PreUpdateMovement()
        {
            if (DBZMOD.Leveled != null)
                LeveledSupport.PlayerPreUpdateMovement(this);
        }

        public override void PostUpdate()
        {
            hitStunManager.Update();
            if (LSSJAchieved && !LSSJ2Achieved && player.whoAmI == Main.myPlayer && IsLegendary() && NPC.downedFishron && player.statLife <= (player.statLifeMax2 * 0.10))
            {
                lssj2Timer++;

                if (lssj2Timer >= 300)
                {
                    if (Main.rand.Next(8) == 0)
                    {
                        Main.NewText("Something uncontrollable is coming from deep inside.", Color.Green);
                        player.statLife = player.statLifeMax2 / 2;
                        player.HealEffect(player.statLifeMax2 / 2);

                        DBZMOD.Instance.TransformationDefinitionManager.LSSJ2Definition.TryUnlock(this);

                        isTransforming = true;
                        LSSJ2Transformation();
                        UI.TransformationMenu.SelectedTransformation = DBZMOD.Instance.TransformationDefinitionManager.LSSJ2Definition;
                        lssj2Timer = 0;
                        player.EndTransformations();
                    }
                    else if (lssj2Timer >= 300)
                    {
                        lssj2Timer = 0;
                        Main.NewText(LSSJ2TextSelect(), Color.Red);
                    }
                }
            }

            if (kiLantern)
            {
                player.AddBuff(mod.BuffType("KiLanternBuff"), 2);
            }
            else
            {
                player.ClearBuff(mod.BuffType("KiLanternBuff"));
            }

            if (isTransforming)
            {
                ssjAuraBeamTimer++;
            }

            if (player.IsPlayerTransformed())
            {
                if (!(player.IsKaioken() && kaiokenLevel == 5) && !player.HasBuff(DBZMOD.Instance.TransformationDefinitionManager.LSSJ2Definition.GetBuffId()))
                {
                    lightningFrameTimer++;
                }
                else
                {
                    lightningFrameTimer += 2;
                }
            }

            if (lightningFrameTimer >= 15)
            {
                lightningFrameTimer = 0;
            }

            //if (!player.IsPlayerTransformed())
            //{
            //    kiDrainAddition = 0;
            //}

            if (player.IsAnyKaioken())
            {
                kaiokenTimer += 1.5f;
            }

            #region Mastery Messages
            if (player.whoAmI == Main.LocalPlayer.whoAmI)
            {
                foreach (var formWithMastery in FormBuffHelper.AllBuffs().Where(x => x.HasMastery).Select(x => x.MasteryBuffKeyName).Distinct())
                {
                    HandleMasteryEvents(formWithMastery);
                }
            }

            #endregion            

            if (adamantiteBonus)
            {
                kiDamage += 7;
            }

            if (PlayerTrait != null && PlayerTrait.BuffName != null)
            {
                if (PlayerTrait.CanSee(this))
                {
                    player.AddBuff(PlayerTrait.GetBuffType(mod), 3);
                    player.ClearBuff(mod.BuffType<UnknownTraitBuff>());
                }
                else player.AddBuff(mod.BuffType<UnknownTraitBuff>(), 3);
            }

            if (IsLegendary() && !LSSJAchieved && NPC.downedBoss1)
            {
                player.AddBuff(mod.BuffType("UnknownLegendary"), 3);
            }
            else if (IsLegendary() && LSSJAchieved)
            {
                player.AddBuff(mod.BuffType("LegendaryTrait"), 3);
                player.ClearBuff(mod.BuffType("UnknownLegendary"));
            }

            if (kiRegen >= 1)
            {
                kiRegenTimer++;
            }

            if (kiRegenTimer > 2)
            {
                AddKi(kiRegen, false, false);
                kiRegenTimer = 0;
            }

            if (demonBonusActive)
            {
                demonBonusTimer++;
                if (demonBonusTimer > 300)
                {
                    demonBonusActive = false;
                    demonBonusTimer = 0;
                    player.AddBuff(mod.BuffType("ArmorCooldown"), 3600);
                }
            }

            if (player.dead && player.IsPlayerTransformed())
            {
                player.EndTransformations();
                isTransforming = false;
            }

            if (rageCurrent > 5)
            {
                rageCurrent = 5;
            }

            HandleOverloadCounters();

            overallFormUnlockChance = formUnlockChance - rageCurrent;

            if (overallFormUnlockChance < 2)
            {
                overallFormUnlockChance = 2;
            }

            if (!player.HasBuff(mod.BuffType("ZenkaiBuff")) && zenkaiCharmActive)
            {
                player.AddBuff(mod.BuffType("ZenkaiCooldown"), 7200);
            }

            if (isDashing)
            {
                player.invis = true;
            }

            HandleBlackFusionMultiplier();

            // neuters flight if the player gets immobilized. Note the lack of Katchin Feet buff.
            if (IsPlayerImmobilized() && isFlying)
            {
                isFlying = false;
            }

            HandleMouseOctantAndSyncTracking();

            // flight system moved to PostUpdate so that it can benefit from not being client sided!
            FlightSystem.Update(player);

            // charge activate and charge effects moved to post update so that they can also benefit from not being client sided.
            HandleChargeEffects();

            // aura frame effects moved out of draw pass to avoid being tied to frame rate!

            currentAura = this.GetAuraEffectOnPlayer();

            // save the charging aura for last, and only add it to the draw layer if no other auras are firing
            if (isCharging)
            {
                if (!wasCharging)
                {
                    var chargeAuraEffects = AuraAnimations.createChargeAura;
                    HandleAuraStartupSound(chargeAuraEffects, true);
                }
            }

            if (currentAura != previousAura)
            {
                auraSoundInfo = SoundHelper.KillTrackedSound(auraSoundInfo);
                HandleAuraStartupSound(currentAura, false);
                // reset aura frame and audio timers to 0, this is important
                auraSoundTimer = 0;
                auraFrameTimer = 0;
            }

            IncrementAuraFrameTimers(currentAura);
            HandleAuraLoopSound(currentAura);

            wasCharging = isCharging;
            previousAura = currentAura;

            CheckPlayerForTransformationStateDebuffApplication();

            ThrottleKi();

            // fires at the end of all the things and makes sure the user is synced to the server with current values, also handles initial state.
            CheckSyncState();

            // Handles nerfing player defense when in Kaioken States
            HandleKaiokenDefenseDebuff();

            // if the player is in mid-transformation, totally neuter horizontal velocity
            // also handle the frame counter advancement here.
            if (isTransformationAnimationPlaying)
            {
                player.velocity = new Vector2(0, player.velocity.Y);

                transformationFrameTimer++;
            }
            else
            {
                transformationFrameTimer = 0;
            }
            KiBar.visible = true;
            if (player.IsCarryingAllDragonBalls() && !wishActive)
            {
                int soundTimer = 0;
                soundTimer++;
                if (soundTimer > 300)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/DBReady"));
                    soundTimer = 0;
                }
            }
        }
    }
}
