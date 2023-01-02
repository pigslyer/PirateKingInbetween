using Godot;

namespace PirateInBetween.Game
{
	public class Hitter : Area2DOverride
	{

		private PhysicsLayers _baseMask;

		public override void _Ready()
		{
			CollisionLayer = PhysicsLayers.None;
			_baseMask = CollisionMask;
			Monitorable = Monitoring = false;
			Connect("body_entered", this, "OnHit");
			Connect("area_entered", this, "OnHit");
		}

		public void SetBaseMask(PhysicsLayers mask)
		{
			_baseMask = mask;
		}

		public async void HitFor(float time, PhysicsLayers? onLayer = null)
		{
			if (onLayer != null)
			{
				this.SetMask((PhysicsLayers) onLayer);
			}
			else if (_baseMask != CollisionMask)
			{
				this.SetMask(_baseMask);
			}

			SetDeferred("monitoring", true);
			SetDeferred("monitorable", true);
			
			await this.AwaitPhysics();

			await this.WaitFor(time);

			SetDeferred("monitoring", false);
			SetDeferred("monitorable", false);
		}

		private void OnHit(object hit)
		{
			if (hit is IHittable target)
			{
				target.Hit(new HitData(1));
			}
		}
	}
}