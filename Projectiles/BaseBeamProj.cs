using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using Util;
using Terraria.Enums;

namespace DBZMOD.Projectiles
{
    // unabashedly stolen from blushie's laser example, and then customized WIP
	public class BaseBeamProj : ModProjectile
    {
        //The distance charge particle from the player center
        private const float TailHeldDistance = 60f;

        // all beams tend to have a similar structure, there's a charge, a tail or "start", a beam (body) and a head (forwardmost point)
        // this is the structure that helps alleviate some of the logic burden by predefining the dimensions of each segment.
        public Point TailOrigin = new Point(14, 0);
        public Point TailSize = new Point(46, 72);
        public Point BeamOrigin = new Point(14, 74);
        public Point BeamSize = new Point(46, 36);
        public Point HeadOrigin = new Point(0, 112);
        public Point HeadSize = new Point(74, 74);
        
        // this determines how long the max fade in for beam opacity takes to fully "phase in", at a rate of 1f per frame. (This is handled by the charge ball)
        public float BeamFadeOutTime = 60f;

        // THIS HAS TO MATCH THE BASE CHARGE PROJ OR IT WILL LOOK STUPID. YOU HAVE BEEN WARNED. Bigger number = slower movement. For reference, 60f is pretty fast.
        public float RotationSlowness = 120f;

        // vector to reposition the beam tail down if it feels too low or too high on the character sprite
        public Vector2 OffsetY = new Vector2(0, 4f);
        
        // the maximum travel distance the beam can go
        public float MaxBeamDistance = 2000f;

        // the speed at which the beam head travels through space
        public float BeamSpeed = 5f;

        // the type of dust to spawn when the beam is firing
        public int DustType = 169;

        // the frequency at which to spawn dust when the beam is firing
        public float DustFrequency = 0.6f;
                
        public Rectangle TailRectangle()
        {
            return new Rectangle(TailOrigin.X, TailOrigin.Y, TailSize.X, TailSize.Y);
        }
                
        public Rectangle BeamRectangle()
        {
            return new Rectangle(BeamOrigin.X, BeamOrigin.Y, BeamSize.X, BeamSize.Y);
        }

        public Rectangle HeadRectangle()
        {
            return new Rectangle(HeadOrigin.X, HeadOrigin.Y, HeadSize.X, HeadSize.Y);
        }

        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable
        public float Distance
        {
            get
            {
                return projectile.ai[0];
            }
            set
            {
                projectile.ai[0] = value;
            }
        }

        public float TrackedDistance
        {
            get
            {
                return projectile.ai[1];
            }
            set
            {
                projectile.ai[1] = value;
            }
        }

        public float DetachmentTimer
        {
            get
            {
                return projectile.localAI[0];
            }
            set
            {
                projectile.localAI[0] = value;
            }
        }

        public float TailDistance
        {
            get
            {
                return projectile.localAI[1];
            }
            set
            {
                projectile.localAI[1] = value;
            }
        }
        
        // the length of a "step" of the body, defined loosely as the body's Y length minus an arbitrary cluster of pixels to overlap cleanly.
        public float StepLength()
        {
            return BeamSize.Y - 4f;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Color.White);
            return false;
        }

        // The core function of drawing a laser
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Color color)
        {   
            // half pi subtracted from the rotation.
            float rotation = projectile.velocity.ToRotation() - 1.57f;
            
            DebugUtil.Log(string.Format("Trying to draw laser tail at {0}, {1}", TailPositionStart().X, TailPositionStart().Y));
            // draw the beam tail
            spriteBatch.Draw(texture, TailPositionStart() - Main.screenPosition, TailRectangle(), color, rotation, new Vector2(TailSize.X * .5f, TailSize.Y * .5f), 1f, 0, 0.96f);
                        
            // draw the body between the beam and its destination point.
            for (float i = 0; i < Distance - StepLength(); i += StepLength())
            {
                Vector2 origin = TailPositionEnd() + i * projectile.velocity;
                spriteBatch.Draw(texture, origin - Main.screenPosition, BeamRectangle(), color, rotation, new Vector2(BeamSize.X * .5f, BeamSize.Y * .5f), 1f, 0, 0.97f);
            }

            DebugUtil.Log(string.Format("Trying to draw laser head at {0}, {1}", BodyPositionEnd().X, BodyPositionEnd().Y));
            // draw the beam head
            // don't make the head transparent or it looks weird.
            spriteBatch.Draw(texture, BodyPositionEnd() - Main.screenPosition, HeadRectangle(), color, rotation, new Vector2(HeadSize.X * .5f, HeadSize.Y * .5f), 1f, 0, 0.98f);
        }
        
        public Vector2 TailPositionStart()
        {
            return projectile.position + OffsetY + TailHeldDistance * projectile.velocity;
        }

        public Vector2 TailPositionEnd()
        {
            return TailPositionStart() + (TailSize.Y - StepLength()) * projectile.velocity;
        }

        public Vector2 BodyPositionEnd()
        {
            return TailPositionEnd() + (Distance - StepLength()) * projectile.velocity;
        }

        public Vector2 HeadPositionEnd()
        {
            return BodyPositionEnd() + HeadSize.Y * projectile.velocity;
        }

        // Change the way of collision check of the projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[projectile.owner];
            Vector2 unit = projectile.velocity;
            float tailPoint = 0f;
            float bodyPoint = 0f;
            float headPoint = 0f;
            
            bool tailCollision = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), TailPositionStart(), TailPositionEnd(), TailSize.X, ref tailPoint);

            bool bodyCollision = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), TailPositionEnd(), BodyPositionEnd(), BeamSize.X, ref bodyPoint);

            bool headCollision = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BodyPositionEnd(), HeadPositionEnd(), HeadSize.X, ref headPoint);

            if (tailCollision)
                ProjectileUtil.DoBeamCollisionDust(DustType, 1.0f, TailPositionStart(), TailPositionEnd(), tailPoint, HeadSize.ToVector2());

            if (bodyCollision)
                ProjectileUtil.DoBeamCollisionDust(DustType, 1.0f, TailPositionEnd(), BodyPositionEnd(), bodyPoint, HeadSize.ToVector2());

            if (headCollision)
                ProjectileUtil.DoBeamCollisionDust(DustType, 1.0f, BodyPositionEnd(), HeadPositionEnd(), headPoint, HeadSize.ToVector2());

            DebugUtil.Log(string.Format("Collision detection Head: {0} Body: {1}", headCollision, bodyCollision));
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return headCollision || bodyCollision;
        }



        // Set custom immunity time on hitting an NPC
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
        }

        public void HandleBeamVisibility()
        {
            var beamTransparency = (int)Math.Min(255f, (1f - (DetachmentTimer / BeamFadeOutTime)) * 255f);
            projectile.alpha = beamTransparency;
        }

        public float GetTransparency()
        {
            return projectile.alpha / 255f;
        }

        // helper field lets us limit mouse movement's impact on the charge ball rotation.
        private Vector2 OldMouseVector = Vector2.Zero;

        // the old screen position helps us offset the MouseWorld vector by our screen position so it's more stable.
        private Vector2 OldScreenPosition = Vector2.Zero;

        // The AI of the projectile
        public override void AI()
        {
            ProcessKillRoutine();
            
            Player player = Main.player[projectile.owner];

            // capture the current mouse vector, we're going to normalize movement prior to updating the charge ball location.
            Vector2 mouseVector = Main.MouseWorld;
            Vector2 screenPosition = Main.screenPosition;
            if (OldMouseVector != Vector2.Zero)
            {
                Vector2 mouseMovementVector = (mouseVector - OldMouseVector) / RotationSlowness;
                Vector2 screenChange = screenPosition - OldScreenPosition;
                mouseVector = OldMouseVector + mouseMovementVector + screenChange;
            }

            UpdateBeamTailLocationAndDirection(player, mouseVector);

            // handle whether the beam should be visible, and how visible.
            HandleBeamVisibility();

            // handle the distance routine
            // the difference between distance and tracked distance is that distance is the actual travel.
            // tracked distance is with collision, and resets distance if it's too high.
            // once released, the distance doesn't increase anymore.
            if (DetachmentTimer == 0)
            {
                Distance = Math.Max(TailHeldDistance, Math.Min(MaxBeamDistance, Distance + BeamSpeed));
                for (TrackedDistance = TailHeldDistance; TrackedDistance <= MaxBeamDistance; TrackedDistance += BeamSpeed)
                {
                    Vector2 origin = TailPositionStart() + projectile.velocity * TrackedDistance;
                    if (!Collision.CanHit(projectile.position, 1, 1, origin, 1, 1))
                    {
                        TrackedDistance -= BeamSpeed;
                        break;
                    }
                }

                // throttle distance by collision
                Distance = Math.Min(TrackedDistance, Distance);
            }

            // shoot sweet sweet particles
            ProjectileUtil.DoBeamDust(projectile.position, projectile.velocity, DustType, DustFrequency, Distance, TailHeldDistance, TailSize.ToVector2(), BeamSpeed);

            //Add lights
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - TailHeldDistance), 26,
                DelegateMethods.CastLight);

            OldMouseVector = mouseVector;
            OldScreenPosition = screenPosition;
        }

        public void ProcessKillRoutine()
        {
            if (DetachmentTimer > 0)
            {
                DetachmentTimer++;
                TailDistance += BeamSpeed;
                Distance -= BeamSpeed;
            }

            if (DetachmentTimer >= BeamFadeOutTime || (DetachmentTimer > 0 && Distance <= 0))
            {
                projectile.Kill();
            }
        }

        public void UpdateBeamTailLocationAndDirection(Player player, Vector2 mouseVector)
        {
            projectile.timeLeft = 2;

            // skip this entire routine if the detachment timer is greater than 0
            if (DetachmentTimer > 0)
                return;

            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (projectile.owner == Main.myPlayer)
            {
                Vector2 diff = mouseVector - player.Center;
                diff.Normalize();
                projectile.velocity = diff;
                projectile.direction = mouseVector.X > player.position.X ? 1 : -1;
                projectile.netUpdate = true;
            }
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();            
            projectile.position = player.Center;
            int dir = projectile.direction;
            player.ChangeDir(dir);
            player.heldProj = projectile.whoAmI;
            if (modPlayer.IsMouseRightHeld)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}