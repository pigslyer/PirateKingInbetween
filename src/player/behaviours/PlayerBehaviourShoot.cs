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
		private const float WAIT_TIME = 1.5f;
		private const float SHOOT_AFTER = 0.7f;

		private const float BULLET_VELOCITY = 100;

		private float _waitedTime;
		private bool _hasShot;

		private PlayerBehaviourModel _model = null!;

		public override void InitializeBehaviour()
		{
			_model = GetSpecificBehaviour<PlayerBehaviourModel>();
		}

		public override void ActiveBehaviour()
		{
			_waitedTime += Delta;

			if (!_hasShot && _waitedTime > SHOOT_AFTER)
			{
				//Damagers.Bullet.GenerateBullet(Controller, Controller.GlobalPosition, new(0,0,0), new Vector2(_model.IsFacingRight ? BULLET_VELOCITY : -BULLET_VELOCITY, 0));
			}

			if (_waitedTime > WAIT_TIME)
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