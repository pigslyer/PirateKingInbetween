using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourJumping : PlayerBehaviour
	{
		public PlayerBehaviourJumping(PlayerController controller) : base(controller)
		{ }

		public override void InitializeBehaviour()
		{ }

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

		public override void PassiveBehaviour()
		{
			if (FrameData.IsOnFloor && InputManager.IsActionJustPressed(InputActions.Jump))
			{
				SetActive();
				VelocityY = BehaviourProperties.JumpVelocity;
			}
		}
	}
}