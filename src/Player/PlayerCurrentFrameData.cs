using Godot;
using System;

using PirateInBetween.Game.Combos;
using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player
{
	public class PlayerCurrentFrameData : ICombatFrameData
	{
		/// <summary>
		/// The time since the last frame.
		/// </summary>
		public float Delta;
		
		/// <summary>
		/// What InputManager.GetMovementVector would return this frame.
		/// </summary>
		/// <returns></returns>
		public readonly Vector2 Input = InputManager.GetMovementVector();

		/// <summary>
		/// Velocity in pixels per second which is to be saved until the next frame.
		/// </summary>
		public Vector2 Velocity;
		
		/// <summary>
		/// The amount by which the movement vector should be multiplied before being applied to the player. Allows for safer speed mults.
		/// </summary>
		public float VelocityMult = 1f;
		
		private PlayerAction _nextAnimation = null;
		
		/// <summary>
		/// What animation the player should use this frame. Can only be changed once, all future changes are ignored.
		/// </summary>
		/// <value></value>
		public PlayerAction CurrentAction
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
		public Animations Animation => CurrentAction.Animation;
		public bool IsBusy => _nextAnimation != null;

		public bool FacingRight = true;


		Vector2 ICombatFrameData.Velocity 
		{ 
			get => Velocity; 
			set => Velocity = value; 
		}

		float ICombatFrameData.Delta => Delta;
		bool ICombatFrameData.FacingRight => FacingRight;
		void ICombatFrameData.SwitchDirection() => FacingRight = !FacingRight;

		CombatAnimation ICombatFrameData.Anim
		{
			set => CurrentAction = new PlayerAction(value);
		}
	}
}