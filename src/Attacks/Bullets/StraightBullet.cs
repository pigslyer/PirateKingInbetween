using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Autoloads;

namespace PirateInBetween.Game
{
	/// <summary>
	/// The bullet is given a target position and speed and by Jove it goes in a straight line from here to there.
	/// </summary>
	public class StraightBullet : DamageDealer, IProjectile<StraightBullet.Data>
	{
		private Vector2 _velocity;
		private DamageData _damageData;
		private Data _movementData;

		public void SetData(DamageData damageData, Data data)
		{
			_damageData = damageData; _movementData = data;
		}

		public void Shoot(Vector2 startingPosition, MovingParent parent)
		{
			ProjectileManager.SetupProjectile<StraightBullet>(this, startingPosition, parent);

			if (_movementData.IsCalculated)
			{
				_velocity = _movementData.Velocity;
			}
			else
			{
				_velocity = (_movementData.TargetPosition - startingPosition).Normalized() * _movementData.Speed;
			}

			Connect(nameof(OnDamageDealt), this, nameof(Destroy));
			Connect(nameof(OnHit), this, nameof(Destroy));
			
			Enable(_damageData);
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			Position += _velocity * delta;
		}

		public void Destroy(Node target)
		{
			QueueFree();
		}

		public class Data
		{
			public readonly bool IsCalculated;

			public readonly Vector2 Velocity;
			public readonly Vector2 TargetPosition;
			public readonly float Speed;
			
			public Data(float speed, Vector2 targetPosition)
			{
				IsCalculated = false;

				Speed = speed;
				TargetPosition = targetPosition;
				
				Velocity = default(Vector2);
			}

			public Data(Vector2 velocity)
			{
				IsCalculated = true;
				
				Velocity = velocity;

				TargetPosition = default(Vector2);
				Speed = default(float);
			}
		}
	}
}