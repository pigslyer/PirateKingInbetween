using System;
using Godot;

namespace PirateInBetween.Game.Player
{
	public class PlayerHorizontalMovement : PlayerBehaviour
	{
		// The maximum speed along the x axis the player can naturally reach.
		[Export] private float _maxSpeed = 200f;

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
			else if (data.Input.x != 0f && Math.Abs(data.Velocity.x) < _maxSpeed)
			{
				float accel = _maxSpeed / _timeToMaxSpeed;
				data.Velocity.x = Mathf.Clamp(
					value : data.Velocity.x + accel * data.Delta * Mathf.Sign(data.Input.x), 
					min : -_maxSpeed, 
					max : _maxSpeed
				);
			}

		}
	}
}