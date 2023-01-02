using Godot;

namespace PirateInBetween.Game
{
	public abstract class BulletBase : KinematicBody2D
	{
		protected Vector2 Velocity;

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