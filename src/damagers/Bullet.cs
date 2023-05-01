using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Pigslyer.PirateKingInbetween.Util.Reflection;
using Pigslyer.PirateKingInbetween.Util.Damage;

namespace Pigslyer.PirateKingInbetween.Damagers
{
	[Path("res://src/damagers/Bullet.tscn")]
	public partial class Bullet : CharacterBody2DOverride
	{
		private static readonly PackedScene _bulletScene = PathAttribute.LoadResource<Bullet>();
		
		private bool _isInitialized = false;
		private DamageData _damageData = null!;
		private BulletData _velocity = null!;

		public static Bullet GenerateBullet(Node parent, Vector2 startingPosition, DamageData data, BulletData velocity)
		{
			Bullet ret = _bulletScene.Instantiate<Bullet>();
			ret._isInitialized = true;
			ret._damageData = data;
			ret._velocity = velocity;

			ret.CollisionLayer = PhysicsLayers2D.None;
			ret.CollisionMask = PhysicsLayers2D.World | data.TargetLayers;

			parent.AddChild(ret);
			ret.GlobalPosition = startingPosition;

			return ret;
		}

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			if (!_isInitialized)
			{
				throw new InvalidOperationException($"{nameof(Bullet)} was never initialized via call to {nameof(GenerateBullet)}!");
			}

			Velocity = _velocity.GetVelocity();

			if (MoveAndSlide())
			{
				var coll = GetSlideCollision(0);

				if (coll.GetCollider() is IDamageTaker taker && taker.CanBeHit(_damageData))
				{
					taker.Hit(_damageData);
				}

				Destroy();
			}
		}

		private void Destroy()
		{
			QueueFree();
		}

		public record BulletDataConstant(Vector2 Velocity) : BulletData()
		{
			public override Vector2 GetVelocity()
			{
				return Velocity;
			}

			public static implicit operator BulletDataConstant(Vector2 Velocity) => new(Velocity);
		}

		public abstract record BulletData()
		{
			public abstract Vector2 GetVelocity();
		}
	}
}