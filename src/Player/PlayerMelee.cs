using Godot;
using System;

namespace PirateInBetween.Player
{
	public class PlayerMelee : PlayerBehaviour
	{
		[Export] private float _meleeAnimationVelocityMult = 0.1f;

		public override void Run(PlayerCurrentFrameData data)
		{
			if (InputManager.IsActionPresseed(Button.MeleeAttack))
			{
				data.NextAnimation = PlayerAnimation.MeleeAttack;
				data.VelocityMult = _meleeAnimationVelocityMult;
			}
		}
	}
}