using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public class PlayerRangedAttack : PlayerBehaviour
	{
		[Export] private ProjectileData _bulletData = null;

		public override void Run(PlayerCurrentFrameData data)
		{
			if (InputManager.IsActionJustPressed(Button.RangedAttack))
			{
				data.AttackData = _bulletData; 
			}
		}
	}
}