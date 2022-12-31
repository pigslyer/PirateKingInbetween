using System;
using Godot;

namespace PirateInBetween.Player
{
	public class PlayerMovement : PlayerBehaviour
	{
		// The maximum speed along the x axis the player can naturally reach.
		[Export] private float _maxSpeed = 200f;

		// Dictates the player's acceleration so they accelerate to above in that time.
		[Export] private float _timeToMaxSpeed = 0.3f;

		// The constant y velocity applied down to the player.
		[Export] private float _gravity = 800f;

		// Maximum height an unassisted jump can reach
		[Export] private float _jumpHeight = 200f;
		// The length of time during which the jump can be cancelled by releasing up.
		[Export] private float _jumpLength = 0.2f;
		// Defines how much of the percentage of the jump * gravity is applied in order to smooth the later parts of the jump.
		[Export] private float _jumpDampening = 0.05f;

		public override void Run(PlayerCurrentFrameData data)
		{
			data.Velocity = RecalculateVelocity(velocity: data.Velocity, delta: data.Delta);

			if (data.Velocity.x != 0f)
			{
				data.FacingRight = data.Velocity.x > 0f;
			}
			
			if (data.Velocity.y > 0f && !Player.IsOnFloor())
			{
				data.NextAnimation = PlayerAnimation.Fall;
			}
			else if (data.Velocity.y < 0f)
			{
				data.NextAnimation = PlayerAnimation.Jump;
			}
			else if (data.Velocity.x != 0f)
			{
				data.NextAnimation = PlayerAnimation.Run;
			}
			else
			{
				data.NextAnimation = PlayerAnimation.Idle;
			}
		}

		private Vector2 RecalculateVelocity(Vector2 velocity, float delta)
		{
			Vector2 ins = InputManager.GetMovementVector();

			velocity = RecalculateHorizontalVelocity(velocity, ins, delta);

			velocity = RecalculateJumpVelocity(velocity, ins, delta);

			return velocity;
		}

		private Vector2 RecalculateHorizontalVelocity(Vector2 velocity, Vector2 ins, float delta)
		{
			// stops sliding when the player wants to change directions/stop
			if (Mathf.Sign(velocity.x) != Mathf.Sign(ins.x) && velocity.x != 0f)
			{
				velocity.x = 0f;
			}

			// accelerates to _maxSpeed
			else if (ins.x != 0f && Math.Abs(velocity.x) < _maxSpeed)
			{
				float accel = _maxSpeed / _timeToMaxSpeed;
				velocity.x = Mathf.Clamp(velocity.x + accel * delta * Mathf.Sign(ins.x), -_maxSpeed, _maxSpeed);
			}

			return velocity;
		}

		private bool _isJumping = false;
		private float _jumpDelta;
		private Vector2 RecalculateJumpVelocity(Vector2 velocity, Vector2 ins, float delta)
		{
			/*
			float JumpLength() => Mathf.Sqrt(2 * _jumpHeight / _gravity);

			float JumpVelocity()
			{
				float t = JumpLength();
				return -(_jumpHeight + _gravity * t * t * 0.5f) / t;
			}
			*/

			float JumpVelocityNoGravity() => -_jumpHeight / _jumpLength;

			// if we're on the floor and holding down jump
			if (!_isJumping && Player.IsOnFloor() && ins.y < 0f)
			{
				velocity.y = JumpVelocityNoGravity();
			
				_isJumping = true;
				_jumpDelta = 0f;
			}
			else
			{
				if (_isJumping)
				{
					_jumpDelta = Mathf.Min(_jumpDelta + delta, _jumpLength);
					velocity.y += _gravity * delta * (_jumpDelta / _jumpLength) * _jumpDampening;

					if (ins.y >= 0f || _jumpDelta >= _jumpLength)
					{
						_isJumping = false;
					}
				}
				else
				{
					if (velocity.y < 0f)
					{
						velocity.y += _gravity * delta;
					}

					velocity.y += _gravity * delta;
				}
			}

			return velocity;
		}
	}
}