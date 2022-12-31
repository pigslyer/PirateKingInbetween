using Godot;
using static Godot.GD;
using System;

namespace PirateInBetween.Player
{
	public class PlayerNoclip : PlayerBehaviour
	{
		[Export] private float _noclipMovementSpeed;

		public override void Run(PlayerCurrentFrameData data)
		{
			data.VelocityMult = 0f;

			Player.Position += InputManager.GetMovementVector() * _noclipMovementSpeed * data.Delta;
		}
	}
}