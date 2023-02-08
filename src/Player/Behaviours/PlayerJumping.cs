using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Behaviours
{
    public class PlayerJumping : PlayerBehaviour
    {
        // Maximum height an unassisted jump can reach
		[Export] private float _jumpHeight = 200f;
		// The length of time during which the jump can be cancelled by releasing up.
		[Export] private float _jumpLength = 0.2f;
		// Defines how much of the percentage of the jump * gravity is applied in order to smooth the later parts of the jump.
		[Export] private float _jumpDampening = 0.05f;
        
        
        private PlayerFalling _fallingBehaviour;
        private float _gravity => _fallingBehaviour.Gravity;

        public override void _Ready()
        {
            _fallingBehaviour = GetSiblingBehaviour<PlayerFalling>(BehavioursPos.Falling);
        }

        /// <summary>
        /// Because of coyote time, jumping, releasing jump in midair and then jumping again causes you to jump a second time. This is here to stop that.
        /// </summary>
        private bool _hasJumped = false;
		private bool _isJumping = false;
		private float _jumpDelta;
        

        public override void Run(PlayerCurrentFrameData data)
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

            
			// if we're on the floor, not doing anything else and holding down jump
			if (CanChangeActive && !_isJumping && !_hasJumped && IsOnFloor() && data.Input.y < 0f)
			{
				data.Velocity.y = JumpVelocityNoGravity();
                
                _hasJumped = _isJumping = true;
                SetBehaviourChangesDisabled(true);
                SetBehavioursEnabled(Behaviours.Falling, false);

				_jumpDelta = 0f;
			}
			else if (_isJumping && IsInControl)
			{
                _jumpDelta = Mathf.Min(_jumpDelta + data.Delta, _jumpLength);
                data.Velocity.y += _gravity * data.Delta * (_jumpDelta / _jumpLength) * _jumpDampening;

                if (data.Input.y >= 0f || _jumpDelta >= _jumpLength)
                {
                    _isJumping = false;
                    SetBehaviourChangesDisabled(false);
                    SetBehavioursEnabled(Behaviours.Falling, true);
                }
            }
            else if (data.Velocity.y < 0f)
            {
                data.Velocity.y += _gravity * data.Delta;
            }


            if (_hasJumped && !IsOnFloor())
            {
                _hasJumped = false;
            }
        }
    }
}