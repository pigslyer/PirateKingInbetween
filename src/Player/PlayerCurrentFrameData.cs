using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public class PlayerCurrentFrameData
	{
		/// <summary>
		/// The time since the last frame.
		/// </summary>
		public readonly float Delta;
		
		/// <summary>
		/// What InputManager.GetMovementVector would return this frame.
		/// </summary>
		/// <returns></returns>
		public readonly Vector2 Input = InputManager.GetMovementVector();

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
		
		private PlayerAnimation? _nextAnimation = null;
		
		/// <summary>
		/// What animation the player should use this frame. Can only be changed once, all future changes are ignored.
		/// </summary>
		/// <value></value>
		public PlayerAnimation? NextAnimation
		{
			get => _nextAnimation;
			set
			{
				if (_nextAnimation == null)
				{
					_nextAnimation = value;
				}
			}
		}

		public bool FacingRight = true;

		public AttackData AttackData = null;
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