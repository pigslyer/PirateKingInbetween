using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class CarriableBox : KinematicBody2DOverride
	{
		[Export] protected float Gravity = 100f;

		[Export] private bool _canBePushed = true;
		[Export] private bool _canBeLifted = true;
		[Export] private bool _canJumpWhileLifting = true;

		#region Paths

		[Export] private NodePath __movingParentDetectorPath = null; 

		#endregion
		
		private bool _isCarried = false;
		private PhysicsLayers _previousLayer;
		private PhysicsLayers _previousMask;

		private float _velocity = 0f;

		private MovingParentDetector _movingParentDetector;

		public override void _Ready()
		{
			_movingParentDetector = GetNode<MovingParentDetector>(__movingParentDetectorPath);
		}

		/// <summary>
		/// Enables/Disables all of the <see cref="CarriableBox"/> behaviour (including physics).
		/// </summary>
		/// <param name="state">True if the box is now going to be carried.</param>
		public void SetCarried(bool state)
		{
			if (state == _isCarried)
			{
				return;
			}

			_isCarried = state;

			if (state)
			{
				_previousLayer = CollisionLayer;
				_previousMask = CollisionMask;

				this.SetLayer(PhysicsLayers.None);
				this.SetMask(PhysicsLayers.None);
			}
			else
			{
				this.SetLayer(_previousLayer);
				this.SetMask(_previousMask);
			}
			
			_movingParentDetector.SetDetecting(!state);
			SetPhysicsProcess(!state);
		}

		public void ApplyVelocity(Vector2 vel)
		{
			MoveAndSlide(vel);
		}

		public virtual bool CanBePushed() => _canBePushed;

		public virtual bool CanBeLifted() => _canBeLifted;

		public virtual bool CanJumpWhileLifting() => _canJumpWhileLifting;

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			_velocity = MoveAndSlide(new Vector2(0, _velocity + Gravity * delta)).y;
		}

		public void SetMovingParent(MovingParent parent) => _movingParentDetector.SetMovingParent(parent);
	}
}