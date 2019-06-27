﻿using DBZMOD.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Projectiles
{
    public class BlindingBladeProj : ModProjectile
    {
        private const int STEP_SPEED = 10;

        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.Homing[projectile.type] = true;
            DisplayName.SetDefault("Blinding Blade");
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 52;
            projectile.timeLeft = 280;
            projectile.penetrate = 16;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.aiStyle = 56; //perfect ai for gravless and rotation, useful for disks 
            projectile.light = 3f;
            projectile.stepSpeed = STEP_SPEED;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            projectile.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }


        public override bool OnTileCollide(Vector2 velocityChange)
        {

            if (projectile.velocity.X != velocityChange.X)
            {
                projectile.velocity.X -= velocityChange.X;
            }
            if (projectile.velocity.Y != velocityChange.Y)
            {
                projectile.velocity.Y -= velocityChange.Y;
            }
            return false;
        }

        public override void PostAI()
        {
            for (int d = 0; d < 1; d++)
            {
                if (Main.rand.NextFloat() < 1f) // always less than 1f?
                {
                    Dust dust = Dust.NewDustDirect(projectile.position, 52, 52, 163, 0f, 0f, 0, new Color(255, 255, 255), 0.7236842f);
                    dust.noGravity = true;
                }

            }
        }

        public override void AI()
        {
            // homing range, projectile acceleration, top speed
            projectile.DoHoming(400f, (float)STEP_SPEED, true);            
        }
    }
}