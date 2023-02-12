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
	[ScenePath("res://src/Attacks/Bullets/StraightBullet.tscn")]
	public class StraightBullet : Projectile
	{
		private bool? _wasGivenPosition = null;
		
		private Vector2 _velocity;
		
		private float _speed;
		private Vector2 _targetPosition;
		
		public void Initialize(PhysicsLayers hitting, DamageAmount amount, float speed, Vector2 targetPosition)
		{
			_wasGivenPosition = true;

			_speed = speed;
			_targetPosition = targetPosition;

			Initialize(hitting, amount);
		}

		public void Initialize(PhysicsLayers hitting, DamageAmount amount, Vector2 velocity)
		{
			_wasGivenPosition = false;

			_velocity = velocity;

			Initialize(hitting, amount);
		}
		
		protected override void OnTreeEntered()
		{
			if (_wasGivenPosition == null)
			{
				throw new InvalidOperationException($"{nameof(StraightBullet)} must have one of its {nameof(Initialize)} methods run before it is shot.");
			}

			if ((bool) _wasGivenPosition)
			{
				_velocity = (_targetPosition - GlobalPosition).Normalized() * _speed;
			}

			GlobalRotation = _velocity.Angle();
		}

		protected override void Move(float delta) => Position += _velocity * delta;

		protected override void Destroy()
		{
			QueueFree();
		}

		protected override bool ShouldHitDestroy(DamageTaker taker) => true;
	}
}