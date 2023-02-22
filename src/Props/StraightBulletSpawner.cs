using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Props
{
	public class StraightBulletSpawner : Node2D
	{
		private PackedScene _bullet = ReflectionHelper.LoadScene<StraightBullet>();

		[Export] private int _damageAmount = 10;
		[Export] private float _stunDuration = 0.7f;

		[Export] private float _firingInterval = 10f;

		[Export] private Vector2 _firingDirection = Vector2.Right;
		[Export] private float _velocity = 100f;

		public override void _Ready()
		{
			base._Ready();

			_firingDirection = _firingDirection.Normalized();
			CallDeferred(nameof(Fire));
		}

		public async void Fire()
		{
			while (!IsQueuedForDeletion())
			{
				StraightBullet bullet = _bullet.Instance<StraightBullet>();
				bullet.Initialize(PhysicsLayers.PlayerHittable | PhysicsLayers.WorldHittable, new DamageAmount(_damageAmount, _stunDuration), _firingDirection * _velocity);
				bullet.Shoot(GlobalPosition, MovingParent.GetMovingParentOf(this));

				await this.WaitFor(_firingInterval);
			}
		}
	}
}