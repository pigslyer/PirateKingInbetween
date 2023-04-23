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

		private float _movementIdleEpsilon => BehaviourProperties.MovementIdleEpsilon;
		private AnimatedSprite2D _sprite => BehaviourProperties.AnimatedSprite;
		
		public override void PassiveBehaviour()
		{
			FrameData.CurrentAnimation = FrameData.Velocity.X > _movementIdleEpsilon ? CharacterAnimation.Walk : CharacterAnimation.Idle;

			if (_sprite.SpriteFrames.HasAnimation(FrameData.CurrentAnimation.ToString()))
			{
				_sprite.Play(FrameData.CurrentAnimation.ToString());
			}
		}
	}
}