using Godot;
using System;
using PirateInBetween.Game.Autoloads;

namespace PirateInBetween.Game
{
	public class HorizontalBullet : BulletBase
	{
		

		public void Setup(Vector2 pos, PhysicsLayers targeting, float speed, bool goingRight)
		{
			ProjectileManager.Instance.Setup(this, pos);
			this.SetMask(targeting | PhysicsLayers.World);
			Velocity = new Vector2(speed * (goingRight ? 1 : -1), 0f);
		}

		protected override void OnHit(KinematicCollision2D hit)
		{
			if (hit.Collider is IHittable target)
			{
				target.Hit(new HitData(1));
			}

			QueueFree();
		}
	}

}