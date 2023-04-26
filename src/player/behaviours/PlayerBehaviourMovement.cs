using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourMovement : PlayerBehaviour
	{
		public PlayerBehaviourMovement(PlayerController playerController) : base(playerController)
		{ }

		private IPlayerBehaviours _notMovemenet = null!;

		public override void InitializeBehaviour()
		{
			_notMovemenet = GetBehaviours(new Type[]{typeof(PlayerBehaviourMovement), typeof(PlayerBehaviourModel)}, true);
		}

		public override void ActiveBehaviour()
		{
			if (InputManager.IsActionPressed(InputActions.Jump))
			{
				VelocityY += BehaviourProperties.JumpPushupAdditional * Delta;
			}

			if (VelocityY >= 0.0f)
			{
				ResetActive();
			}

			FrameData.CurrentAnimation = CharacterAnimation.Jump;
		}


		private float _accel => BehaviourProperties.AccelerationRate;
		private float _deaccel => BehaviourProperties.DeaccelerationRate;
		private float _maxSpeed => BehaviourProperties.MaximumVelocity;

		private float _gravity => BehaviourProperties.Gravity;
		private float _gravityPushDown => BehaviourProperties.GravityPushDownAdditional;

		public override void PassiveBehaviour()
		{
			if (!_notMovemenet.IsAnyActive())
			{
				// ----------------------------------------------------------------------------
				// horizontal movement

				float diff = InputManager.GetDirectionDiff(InputActions.MoveRight, InputActions.MoveLeft);

				// we're moving to a stop
				if (diff == 0.0f)
				{
					VelocityX = Mathf.MoveToward(VelocityX, 0.0f, _deaccel * Delta);
				}
				// we're accelerating in the opposite direction
				else if (Mathf.Sign(diff) != Mathf.Sign(VelocityX))
				{
					VelocityX += _accel * Delta * diff;
				}
				// we're accelerating in the same direction
				else if (Mathf.Abs(FrameData.Velocity.X) < _maxSpeed)
				{
					// this notably doesn't clamp velocity
					VelocityX += Mathf.Min(_accel * Delta, _maxSpeed - Mathf.Abs(VelocityX)) * diff;
				}

				// ----------------------------------------------------------
				// falling

				VelocityY += _gravity * Delta;

				if (!IsActive && InputManager.IsActionPressed(InputActions.Down))
				{
					VelocityY += _gravityPushDown * Delta;
				}

				if (!IsActive && !FrameData.IsOnFloor)
				{
					FrameData.CurrentAnimation = CharacterAnimation.Fall;
				}

				// -----------------------------------------------------------
				// jumpin
				
				if (FrameData.IsOnFloor && InputManager.IsActionJustPressed(InputActions.Jump))
				{
					SetActive();
					VelocityY = BehaviourProperties.JumpVelocity;
				}
			}
		}
	}
}