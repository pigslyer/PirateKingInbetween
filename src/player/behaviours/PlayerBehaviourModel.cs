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
		private bool _usingWoodLeg => BehaviourProperties.UsingWoodLeg;
		private AnimatedSprite2D _sprite => BehaviourProperties.AnimatedSprite;
		
		public override void PassiveBehaviour()
		{
			string GetBasicAnimation(CharacterAnimation fromAnim)
			{
				return fromAnim switch
				{
					CharacterAnimation.Idle => "idleAnimation",
					CharacterAnimation.Run => "runningAnimation",
					CharacterAnimation.Jump => "jumpingAnimation",
					CharacterAnimation.Fall => "fallAnimation",
					_ => "idleAnimation"
				};
			}

			string GetAnimation(CharacterAnimation fromAnim)
			{
				return $"{GetBasicAnimation(fromAnim)}_{(_usingWoodLeg ? "woodenLeg" : "normal")}_{(IsFacingRight ? "right" : "left")}";
			}

			Controller.CurrentAnimation = Mathf.Abs(Controller.Velocity.X) > _movementIdleEpsilon ? CharacterAnimation.Run : CharacterAnimation.Idle;

			if (VelocityX != 0.0f)
			{
				IsFacingRight = VelocityX >= 0.0f;
			}

			string curAnim = GetAnimation(Controller.CurrentAnimation);
			if (_sprite.SpriteFrames.HasAnimation(curAnim) && _sprite.Animation != curAnim)
			{
				_sprite.Play(curAnim);
			}

			

		}
	}
}