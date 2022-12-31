using Godot;
using System;

namespace PirateInBetween.Player
{
	public class PlayerCurrentFrameData
	{
		/// <summary>
		/// The time since the last frame.
		/// </summary>
		public readonly float Delta;

		public PlayerCurrentFrameData(float delta)
		{
			Delta = delta;
		}

		/// <summary>
		/// Velocity in pixels per second which is to be saved until the next frame.
		/// </summary>
		public Vector2 Velocity;
		/// <summary>
		/// The amount by which the movement vector should be multiplied before being applied to the player. Allows for safer speed mults.
		/// </summary>
		public float VelocityMult = 1f;
		
		public PlayerAnimation NextAnimation;
		public bool FacingRight = true;
	}

	public enum PlayerAnimation
	{
		Idle,
		Jump,
		Fall,
		Run,
		MeleeAttack,
		RangedAttack,
	}
}