using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Enemies.Behaviours
{
	public abstract class GroundlingBehaviour : BasicBehaviour
	{
		#region Paths
		[Export] private NodePath __floorFacingRayCastPath = null;
		#endregion

		private const float COYOTE_TIME = 0.3f;
		[Export] protected float PatrolMovementAcceleration = 200f;
		[Export] protected float PatrolMaxSpeed = 100f;
		[Export] protected float WallSightRange { get; private set; } = 100f;
		[Export] private float _gravity = 800f;
		[Export] private float _stunnedOnFloorDeaccel = 200f;

		private RayCast2D _floorFacingRayCast = null; 

		protected override void Run(out bool mayCallExecute)
		{
			base.Run(out mayCallExecute);

			RunCoyoteTime();
			
			ApplyGravity();

			mayCallExecute = mayCallExecute && IsOnFloor();
		}

		#region Overrides

		public override void _Ready()
		{
			base._Ready();

			_floorFacingRayCast = GetNode<RayCast2D>(__floorFacingRayCastPath);
		}


		protected override void WhenStunned()
		{
			base.WhenStunned();

			if (IsOnFloor() && IsStunned())
			{
				CurrentData.Velocity.x = CurrentData.Velocity.x.MoveTowards(0, _stunnedOnFloorDeaccel * CurrentData.Delta);
			}
		}

		public override void OnKnockedback(Vector2 velocity, EnemyFrameData data)
		{
			base.OnKnockedback(velocity, data);

			if (velocity.y < 0f)
			{
				NotOnFloor();
			}
		}

		#endregion

		#region Internal methods

		#region Wandering

		private float _remainingDuration = -1;
		private bool _isWaiting = true;

		protected bool IsCurrentlyWaiting() => _isWaiting;

		protected bool IsWanderingOrWaiting() => _remainingDuration > 0;

		protected void StopWandering(int standingTime)
		{
			_isWaiting = true;
			_remainingDuration = standingTime;
		}

		protected bool Wander(float maxSpeed, float accel, FloatInterval walkingDuration, FloatInterval standingDuration, bool continuePatrol)
		{
			if (continuePatrol && (_remainingDuration <= 0 || (!_isWaiting && (IsAboutToSeeWall() || IsAboutToFallOffEdge()))))
			{
				_isWaiting = !_isWaiting;
				_remainingDuration = (_isWaiting ? standingDuration : walkingDuration).GetRandom();

				if (_remainingDuration <= 0f)
				{
					Wander(maxSpeed, accel, walkingDuration, standingDuration, true);
					return false;
				}

				// turn around if we're starting to walk into a wall
				if (!_isWaiting && (CanSeeWall() || IsAboutToFallOffEdge()))
				{
					CurrentData.FacingRight = !CurrentData.FacingRight;
				}
				
				else if (!_isWaiting)
				{
					CurrentData.FacingRight = GD.Randi() % 2 == 0;
				}
			}

			if (!IsWanderingOrWaiting())
			{
				return false;
			}

			if (_isWaiting)
			{
				CurrentData.Velocity.x = CurrentData.Velocity.x.MoveTowards(0, accel * CurrentData.Delta);
			}
			else
			{
				CurrentData.Velocity.x = CurrentData.Velocity.x.MoveTowards(maxSpeed * CurrentData.FacingRight.Sign(), accel * CurrentData.Delta);
			}

			_remainingDuration -= CurrentData.Delta;

			return false;
		}

		#endregion

		protected void ApplyGravity() => CurrentData.Velocity.y += _gravity * CurrentData.Delta;

		protected bool IsAboutToSeeWall() => CanSeeWall(Controller.GlobalPosition.MoveX(CurrentData.Velocity.x * CurrentData.Delta), CurrentData.FacingRight, WallSightRange + EnemyWidth);

		protected override bool CanSeeWall() => CanSeeWall(Controller.GlobalPosition, CurrentData.FacingRight, WallSightRange + EnemyWidth);

		protected bool CanSeeWall(Vector2 from, bool facingRight, float sightRange)
		{
			if (!TryGetFloorTileMap(out TileMap map))
			{
				return base.CanSeeWall();
			}

			Vector2 lookFrom = Controller.GlobalPosition + new Vector2(EnemyWidth * facingRight.Sign(), 0f);

			float? distance = map.GetDistanceToWall(lookFrom, facingRight);

			if (distance is float distanceToWall)
			{
				return sightRange > distanceToWall;
			}

			return base.CanSeeWall();
		}

		/// <summary>
		/// <para>
		/// A helper method which determines, based on the given position, deacceleration speed, velocity and facing, 
		/// whether or not an unmodified velocity vector will lead to this controller falling off an edge after 1 frame.
		/// </para>
		/// <para>
		/// All parameters default to their most appropriate counterparts as defined in <see cref="GroundlingBehaviour"/>.
		/// </para>
		/// </summary>
		/// <param name="deaccel"></param>
		/// <param name="velocity"></param>
		/// <param name="currentPosition"></param>
		/// <param name="facingRight"></param>
		/// <param name="delta"></param>
		/// <returns></returns>
		protected bool IsAboutToFallOffEdge(float? deaccel = null, Vector2? velocity = null, Vector2? currentPosition = null, bool? facingRight = null, float? delta = null) => IsAboutToFallOffEdgeInternal(deaccel ?? PatrolMovementAcceleration, velocity ?? CurrentData.Velocity, currentPosition ?? Controller.GlobalPosition, facingRight ?? CurrentData.FacingRight, delta ?? CurrentData.Delta);

		private bool IsAboutToFallOffEdgeInternal(float deaccel, Vector2 velocity, Vector2 currentPosition, bool facingRight, float delta)
		{
			return WillFallOffEdgeInternal(deaccel, velocity, currentPosition + new Vector2(velocity.x * delta * facingRight.Sign(), 0f), facingRight);
		}

		/// <summary>
		/// <para>
		/// A helper method which determines, based on the given position, deacceleration speed, velocity and facing,
		/// whether or not an unmodified velocity vector would lead to this controller falling off an edge.
		/// </para>
		/// All parameters default to their most appropriate counterparts as defined in <see cref="GroundlingBehaviour"/>.
		/// </summary>
		/// <param name="deaccel"></param>
		/// <param name="velocity"></param>
		/// <param name="currentPosition"></param>
		/// <param name="facingRight"></param>
		/// <returns></returns>
		protected bool WillFallOffEdge(float? deaccel = null, Vector2? velocity = null, Vector2? currentPosition = null, bool? facingRight = null) => WillFallOffEdgeInternal(deaccel ?? PatrolMovementAcceleration, velocity ?? CurrentData.Velocity, currentPosition ?? Controller.GlobalPosition, facingRight ?? CurrentData.FacingRight);

		private bool WillFallOffEdgeInternal(float deaccel, Vector2 velocity, Vector2 currentPosition, bool facingRight)
		{
			if (new Vector2(0,0).LengthSquared() == 0) return false;

			if (!IsOnFloor())
			{
				return true;
			}

			if (!TryGetFloorTileMap(out TileMap map))
			{
				return false;
			}

			currentPosition += new Vector2(EnemyWidth, 0f) * facingRight.Sign();

			float? distance = map.GetDistanaceToEdge(currentPosition, facingRight);

			if (distance is float distanceToEdge)
			{
				return GetDistanceIfStoppingWith(deaccel, velocity.x) > distanceToEdge;
			}

			return true;
		}

		protected float GetDistanceIfStoppingWith(float deaccel, float currentVelocity)
		{
			float timeToStop = currentVelocity / deaccel;
			return currentVelocity * timeToStop - deaccel * timeToStop.Sqr() * 0.5f;
		}

		protected bool TryGetFloorTileMap(out TileMap tilemap)
		{
			tilemap = _floorFacingRayCast.GetCollider() as TileMap;
			return tilemap != null;
		}

		#region Flooring
		private float _onFloorTimer = 0f;

		private void RunCoyoteTime()
		{
			if (Controller.IsOnFloor() && CurrentData.Velocity.y >= 0f)
			{
				_onFloorTimer = 0f;
			}
			else
			{
				_onFloorTimer += CurrentData.Delta;
			}
		}

		protected void NotOnFloor() => _onFloorTimer = float.PositiveInfinity;

		protected bool IsOnFloor() => _onFloorTimer < COYOTE_TIME;

		#endregion

		#endregion

	}
}