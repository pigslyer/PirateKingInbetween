using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public class PlayerRangedAttack : PlayerBehaviour
	{
		[Export] private PackedScene _bulletScene;

		public override void Run(PlayerCurrentFrameData data)
		{
			if (InputManager.IsActionJustPressed(Button.RangedAttack))
			{
				
			}
		}
	}
}