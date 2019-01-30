﻿using DBZMOD.Extensions;
using DBZMOD.Util;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PlayerExtensions = DBZMOD.Extensions.PlayerExtensions;

namespace DBZMOD.Projectiles.AuraProjectiles
{
    public class SSJAuraBall : ModProjectile
    {
        private float _sizeTimer;
        private float _blastTimer;
        public override void SetDefaults()
        {
            projectile.width = 176;
            projectile.height = 177;
            projectile.aiStyle = 0;
            projectile.timeLeft = 200;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.damage = 0;
            _sizeTimer = 0f;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (MyPlayer.ModPlayer(player).IsPlayerLegendary())
            {
                projectile.position.X = player.Center.X;
                projectile.position.Y = player.Center.Y;
                projectile.Center = player.Center + new Vector2(0, -25);
                projectile.netUpdate = true;

                if (!MyPlayer.ModPlayer(player).isTransforming)
                {
                    projectile.Kill();
                }

                if (_sizeTimer < 200)
                {
                    projectile.scale = _sizeTimer / 50f * 4;
                    _sizeTimer++;
                }

            }
            else if(!MyPlayer.ModPlayer(player).IsPlayerLegendary())
            {
                projectile.position.X = player.Center.X;
                projectile.position.Y = player.Center.Y;
                projectile.Center = player.Center + new Vector2(0, -25);
                projectile.netUpdate = true;

                if (!MyPlayer.ModPlayer(player).isTransforming)
                {
                    projectile.Kill();
                }
                if (_sizeTimer < 300)
                {
                    projectile.scale = _sizeTimer / 300f * 4;
                    _sizeTimer++;
                }
                else
                {
                    projectile.scale = 1f;
                }
                projectile.frameCounter++;
                if (projectile.frameCounter > 8)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 4)
                {
                    projectile.frame = 0;
                }
                if (projectile.active)
                {
                    _blastTimer++;
                    if (_blastTimer > 1)
                    {
                        Vector2 velocity = Vector2.UnitY.RotateRandom(MathHelper.TwoPi) * 30;
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, velocity.X, velocity.Y, mod.ProjectileType("SSJEnergyBarrageProj"), 0, 0, player.whoAmI);
                        _blastTimer = 0;
                    }

                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            if (!MyPlayer.ModPlayer(player).IsPlayerLegendary())
            {
                player.DoTransform(FormBuffHelper.ssj2, DBZMOD.instance);
                MyPlayer.ModPlayer(player).isTransforming = false;
            }
            else
            {
                Projectile.NewProjectile(player.Center.X - 40, player.Center.Y + 90, 0, 0, mod.ProjectileType("LSSJAuraBall"), 0, 0, player.whoAmI);
                // this being set to false prior to the aura ball dying tells it to go LSSJ instead of LSSJ2 - weird choice, but I'm not going to argue with it.
                MyPlayer.ModPlayer(player).isTransforming = false;
            }
        }
    }
}
