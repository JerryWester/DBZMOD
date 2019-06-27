﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using DBZMOD.Util;

namespace DBZMOD.Projectiles
{
    public class SuperEnergyBarrageProj : KiProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 20;
            projectile.light = 1f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
            projectile.aiStyle = 1;
            aiType = 14;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            projectile.netUpdate = true;
        }
		
		public override Color? GetAlpha(Color lightColor)
        {
			if (projectile.timeLeft < 85) 
			{
				byte b2 = (byte)(projectile.timeLeft * 3);
				byte a2 = (byte)(100f * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			return new Color(255, 255, 255, 100);
        }
		
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 0, 0, 0, mod.ProjectileType("SuperEnergyBarrageExplosion"), projectile.damage, 4f, projectile.owner, 0, projectile.rotation);

            SoundHelper.PlayCustomSound("Sounds/Kiplosion", projectile.position, 1.0f);
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
    }
}