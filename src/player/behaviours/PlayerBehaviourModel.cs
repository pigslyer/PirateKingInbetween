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
		public bool IsFacingRight { get; private set; } = true;

		public override void ActiveBehaviour()
		{ }

		private float _movementIdleEpsilon => BehaviourProperties.MovementIdleEpsilon;
		private AnimatedSprite2D _sprite => BehaviourProperties.AnimatedSprite;
		
		public override void PassiveBehaviour()
		{
			Controller.CurrentAnimation = Mathf.Abs(Controller.Velocity.X) > _movementIdleEpsilon ? CharacterAnimation.Walk : CharacterAnimation.Idle;

			if (_sprite.SpriteFrames.HasAnimation(Controller.CurrentAnimation.ToString()) && _sprite.Animation != Controller.CurrentAnimation.ToString())
			{
				_sprite.Play(Controller.CurrentAnimation.ToString());
			}

			if (VelocityX != 0.0f)
			{
				IsFacingRight = VelocityX >= 0.0f;
				_sprite.FlipH = !IsFacingRight;
			}

		}
	}
}