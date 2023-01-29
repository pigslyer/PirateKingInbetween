using System;
using Godot;

namespace PirateInBetween.Game.Player.Behaviours
{
	public class PlayerHorizontalMovement : PlayerBehaviour
	{
		// The maximum speed along the x axis the player can naturally reach.
		[Export] public float MaxSpeed { get; private set; } = 200f;

		// Dictates the player's acceleration so they accelerate to above in that time.
		[Export] private float _timeToMaxSpeed = 0.3f;

		public override void Run(PlayerCurrentFrameData data)
		{
			// stops sliding when the player wants to change directions/stop
			if (Mathf.Sign(data.Velocity.x) != Mathf.Sign(data.Input.x) && data.Velocity.x != 0f)
			{
				data.Velocity.x = 0f;
			}

			// accelerates to _maxSpeed
			else if (data.Input.x != 0f && Math.Abs(data.Velocity.x) < MaxSpeed)
			{
				float accel = MaxSpeed / _timeToMaxSpeed;
				data.Velocity.x = Mathf.Clamp(
					value : data.Velocity.x + accel * data.Delta * Mathf.Sign(data.Input.x), 
					min : -MaxSpeed, 
					max : MaxSpeed
				);
			}

		}
	}
}