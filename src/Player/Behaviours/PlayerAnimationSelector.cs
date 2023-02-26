using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Behaviours
{
    public class PlayerAnimationSelector : PlayerBehaviour
    {
        public override void Run(PlayerCurrentFrameData data)
        {
			if (data.CurrentAction == null)
			{
				var animationData = GetDefaultAnimation(data);
				data.FacingRight = animationData.FacingRight;
				data.CurrentAction = animationData.Animation;
			}
		}

		public (bool FacingRight, Animations Animation) GetDefaultAnimation(PlayerCurrentFrameData data)
		{
			bool facing = data.FacingRight;
			Animations animation;

			if (data.Velocity.x != 0f)
			{
				facing = data.Velocity.x > 0f;
			}
			
			if (data.Velocity.y > 0f && !IsOnFloor())
			{
				animation = Animations.Fall;
			}
			else if (data.Velocity.y < 0f)
			{
				animation = Animations.Jump;
			}
			else if (data.Velocity.x != 0f)
			{
				animation = Animations.Run;
			}
			else
			{
				animation = Animations.Idle;
			}

			return (facing, animation);
        }
    }
}