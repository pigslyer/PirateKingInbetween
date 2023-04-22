using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourModel : PlayerBehaviour
	{
		public PlayerBehaviourModel(PlayerController playerController) : base(playerController)
		{
			
		}

		public override void InitializeBehaviour()
		{

		}

		public override void ActiveBehaviour()
		{ }

		private float MovementIdleEpsilon => BehaviourProperties.MovementIdleEpsilon;

		public override void PassiveBehaviour()
		{
			FrameData.CurrentAnimation = FrameData.Velocity.X > MovementIdleEpsilon ? CharacterAnimation.Walk : CharacterAnimation.Idle;

			
		}
	}
}