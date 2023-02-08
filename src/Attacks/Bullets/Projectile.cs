using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{

	public abstract class Projectile : DamageDealer
	{ 
		private DamageData _damageData;

		protected void Initialize(PhysicsLayers hitting, DamageAmount amount)
		{
			_damageData = new DamageData(amount, () => GlobalPosition);
			
			CollisionLayer = PhysicsLayers.None;
			CollisionMask = hitting;
		}

		public override void _PhysicsProcess(float delta)
		{
			Move(delta);
		}

		protected abstract void Move(float delta);

		private void OnHitDestroy(Node hitWhat)
		{
			if (hitWhat is DamageTaker taker && !ShouldHitDestroy(taker))
			{
				return;
			}

			Disable();
			Destroy();
		}

		protected abstract void Destroy();

		protected abstract bool ShouldHitDestroy(DamageTaker taker);


		public void Shoot(Vector2 startingPosition, MovingParent parent)
		{
			if (_damageData == null)
			{
				throw new InvalidOperationException($"{GetType()} never called internal {nameof(Initialize)} method with {nameof(DamageAmount)}.");
			}

			Position = startingPosition;
			
			
			parent.Move(this);

			Connect(nameof(OnDamageDealt), this, nameof(OnHitDestroy));
			Connect(nameof(OnHit), this, nameof(OnHitDestroy));

			Enable(_damageData);
			
			OnTreeEntered();
		}

		protected abstract void OnTreeEntered();
	}
}