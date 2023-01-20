using Godot;

namespace PirateInBetween.Game
{
	public abstract class BulletBase : KinematicBody2DOverride, IProjectile
	{
		protected Vector2 Velocity;

		public abstract void AimAt(Vector2 startFrom, Node2D target, ProjectileData data, PhysicsLayers targetting);

		public override void _PhysicsProcess(float delta)
		{
			var data = MoveAndCollide(Velocity * delta);

			if (data != null)
			{
				OnHit(data);
			}
		}

		protected abstract void OnHit(KinematicCollision2D hit);
	}
}