using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourHorizontalMovement : PlayerBehaviour
	{
		public PlayerBehaviourHorizontalMovement(PlayerController player) : base(player)
		{  }

		public override void InitializeBehaviour()
		{ }

		public override void ActiveBehaviour()
		{ }

		private float _accel => BehaviourProperties.AccelerationRate;
		private float _deaccel => BehaviourProperties.DeaccelerationRate;
		private float _maxSpeed => BehaviourProperties.MaximumVelocity;

		public override void PassiveBehaviour()
		{
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
		}
	}
}