using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public class PlayerMelee : PlayerBehaviour
	{
		[Export] private float _meleeAnimationVelocityMult = 0.1f;
		[Export] private SlashData _data = null;

		public override void Run(PlayerCurrentFrameData data)
		{
			if (InputManager.IsActionPresseed(InputButton.MeleeAttack))
			{
				data.NextAnimation = PlayerAnimation.MeleeAttack;
				data.VelocityMult = _meleeAnimationVelocityMult;
				data.AttackData = _data;
			}
		}
	}
}