using Godot;
using System;
using PirateInBetween.Game.Autoloads;

namespace PirateInBetween.Game
{
	public class HorizontalBullet : BulletBase
	{
		[Export] private float _speed = 100f;
		private ProjectileData _data;

		public override void AimAt(Vector2 startFrom, Node2D target, ProjectileData data, PhysicsLayers targetting)
		{
			_data = data;
			ProjectileManager.Instance.Setup(this, startFrom);
			this.SetMask(targetting | PhysicsLayers.World);
			Velocity = new Vector2(_speed * Mathf.Sign(target.GlobalPosition.x - startFrom.x), 0f);
		}

		protected override void OnHit(KinematicCollision2D hit)
		{
			if (hit.Collider is IHittable target)
			{
				target.Hit(new HitData(_data.Damage));
			}

			QueueFree();
		}
	}

}