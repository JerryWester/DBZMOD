using Microsoft.Xna.Framework;

namespace DBZMOD.Projectiles
{
	public class DoubleSundayBeam : BaseBeam
	{
		public override void SetDefaults()
        {
            // all beams tend to have a similar structure, there's a charge, a tail or "start", a beam (body) and a head (forwardmost point)
            // this is the structure that helps alleviate some of the logic burden by predefining the dimensions of each segment.
            tailOrigin = new Point(0, 0);
            tailSize = new Point(106, 92);
            beamOrigin = new Point(18, 94);
            beamSize = new Point(70, 2);
            headOrigin = new Point(18, 98);
            headSize = new Point(70, 14);

            // this determines how long the max fade in for beam opacity takes to fully "phase in", at a rate of 1f per frame. (This is handled by the charge ball)
            beamFadeOutTime = 30f;

            // Bigger number = slower movement. For reference, 60f is pretty fast. 180f is pretty slow.
            rotationSlowness = 60f;

            // vector to reposition the beam tail down if it feels too low or too high on the character sprite
            offsetY = new Vector2(0, 4f);

            // the maximum travel distance the beam can go
            maxBeamDistance = 2000f;

            // the speed at which the beam head travels through space
            beamSpeed = 15f;

            // the type of dust to spawn when the beam is firing
            dustType = 27;

            // the frequency at which to spawn dust when the beam is firing
            dustFrequency = 0.6f;

            // how many particles per frame fire while firing the beam.
            fireParticleDensity = 6;

            // the frequency at which to spawn dust when the beam collides with something
            collisionDustFrequency = 1.0f;

            // how many particles per frame fire when the beam collides with something
            collisionParticleDensity = 8;

            // The sound effect used by the projectile when firing the beam. (plays on initial fire only)
            beamSoundKey = "Sounds/BasicBeamFire";

            isEntityColliding = true;

            base.SetDefaults();
        }
	}
}