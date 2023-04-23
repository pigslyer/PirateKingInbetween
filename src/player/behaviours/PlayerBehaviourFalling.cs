using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourFalling : PlayerBehaviour
	{
		public PlayerBehaviourFalling(PlayerController controller) : base(controller)
		{ }


		private IPlayerBehaviours _jumpingBehaviour = null!;
		public override void InitializeBehaviour()
		{ 
			_jumpingBehaviour = GetBehaviours(
				new Type[]
				{
					typeof(PlayerBehaviourJumping),
				}
			);
		}		

		private float _gravity => BehaviourProperties.Gravity;
		private float _gravityPushDown => BehaviourProperties.GravityPushDownAdditional;

		public override void PassiveBehaviour()
		{
			VelocityY += _gravity * Delta;

			if (!_jumpingBehaviour.IsAnyActive() && InputManager.IsActionPressed(InputActions.Down))
			{
				VelocityY += _gravityPushDown * Delta;
			}
		}

		public override void ActiveBehaviour()
		{ }

	}
}