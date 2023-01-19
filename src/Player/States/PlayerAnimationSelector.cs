using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player
{
    public class PlayerAnimationSelector : PlayerBehaviour
    {
        public override void Run(PlayerCurrentFrameData data)
        {
            
			if (data.Velocity.x != 0f)
			{
				data.FacingRight = data.Velocity.x > 0f;
			}
			
			if (data.Velocity.y > 0f && !IsOnFloor())
			{
				data.NextAnimation = PlayerAnimation.Fall;
			}
			else if (data.Velocity.y < 0f)
			{
				data.NextAnimation = PlayerAnimation.Jump;
			}
			else if (data.Velocity.x != 0f)
			{
				data.NextAnimation = PlayerAnimation.Run;
			}
			else
			{
				data.NextAnimation = PlayerAnimation.Idle;
			}
        }
    }
}