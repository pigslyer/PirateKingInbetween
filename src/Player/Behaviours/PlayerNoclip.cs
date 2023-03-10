using Godot;
using static Godot.GD;
using System;

namespace PirateInBetween.Game.Player.Behaviours
{
	public class PlayerNoclip : PlayerBehaviour
	{
		[Export] private float _noclipMovementSpeed = 100f;

		public override void Run(PlayerCurrentFrameData data)
		{
			data.VelocityMult = 0f;
			data.CurrentAction = Animations.Idle;

			GetPlayer().Position += data.Input * _noclipMovementSpeed * data.Delta;
		}
	}
}