using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourShoot : PlayerBehaviour
	{
		private PlayerBehaviourModel _model = null!;

		public override void InitializeBehaviour()
		{
			_model = GetSpecificBehaviour<PlayerBehaviourModel>();
		}

		private float _waitTime => BehaviourProperties.ShootTotalWaitTime;
		private float _shootAfter => BehaviourProperties.ShootAfterTime;
		private float _bulletVelocity => BehaviourProperties.ShootBulletVelocity;

		private float _waitedTime;
		private bool _hasShot;

		public override void ActiveBehaviour()
		{
			_waitedTime += Delta;

			if (!_hasShot && _waitedTime > _shootAfter)
			{
				Damagers.Bullet.GenerateBullet(
					Controller.GetParent(), 
					BehaviourProperties.ShootFromPosition.GlobalPosition, 
					new(PhysicsLayers2D.World), 
					new Damagers.Bullet.BulletDataConstant(
						(_model.IsFacingRight ? _bulletVelocity : -_bulletVelocity).VecX0()
					)
				);

				_hasShot = true;
			}

			if (_waitedTime > _waitTime)
			{
				_waitedTime = 0.0f;
				ResetActive();
			}	
		}

		private void PreActive()
		{
			VelocityX = 0;
			_waitedTime = 0.0f;
			_hasShot = false;
		}

		public override void PassiveBehaviour()
		{
			if (!IsAnyActive() && Controller.IsOnFloor() && InputManager.IsActionJustPressed(InputActions.Shoot))
			{
				PreActive();
				SetActive();
			}
		}
	}
}