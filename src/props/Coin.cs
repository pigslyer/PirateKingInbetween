using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Pigslyer.PirateKingInbetween.Util.Reflection;

namespace Pigslyer.PirateKingInbetween.Props
{
	[Path("res://src/props/Coin.tscn")]
	public partial class Coin : Node2D
	{

		private const float COIN_SPAWN_MAX_RADIUS = 5f;

		public static void SpawnCoins(Vector2 globalPosition, Node2D flyTowards, int count)
		{
			PackedScene scene = PathAttribute.LoadResource<Coin>();
			float baseAngle = (globalPosition - (flyTowards is IHasCenterOfMass mass ? mass.CenterOfMass : flyTowards.GlobalPosition)).Angle();

			for (int i = 0; i < count; i++)
			{
				Coin inst = scene.Instantiate<Coin>();

				float angle = baseAngle + (-0.5f + GD.Randf()) * Mathf.Pi;
				
				inst.Position = globalPosition;
				inst.ZIndex = 1000;
				Projectile.AddChild(inst);

				inst.SetData(flyTowards, angle);
			}
		}
		
		private void SetData(Node2D following, float angle)
		{
			_following = following;
			_centerOfMass = following as IHasCenterOfMass;

			_velocity = new Vector2(LINEAR_VELOCITY_SPEED, 0).Rotated(angle);
		}

		enum States
		{
			Starting,
			Flying,
			Ending,
		};
		
		private States _curState = States.Starting;
		private Node2D _following = null!;
		private IHasCenterOfMass? _centerOfMass;
		private Vector2 _velocity;

		public override void _Process(double delta)
		{
			base._Process(delta);

			float fdelta = (float)delta;

			switch (_curState)
			{
				case States.Starting:
					StartingProcess(fdelta);

					break;

				case States.Flying:
					FlyingProcess(fdelta);

					break;

				case States.Ending:
					EndingProcess(fdelta);

					break;
			}
		}
		
		private Vector2 _targetPosition => _centerOfMass?.CenterOfMass ?? _following.GlobalPosition;

		// ----------------------------------------------------
		// STARTUP STATE CONSTANTS
		private const float STARTUP_TIME = 0.3f;
		private const float FINAL_SCALE = 2.0f;

		// ----------------------------------------------------
		// FLYING STATE CONSTANTS
		private const float LINEAR_VELOCITY_SPEED = 10.0f;
		private const float LINEAR_ACCELERATION = 120.0f;
		private const float MAX_VELOCITY = 10.0f;
		private const float ENDING_DISTANCE = 23.0f;
		private float ENDING_DISTANCE_SQR => ENDING_DISTANCE * ENDING_DISTANCE;
		private const float MAX_FLYING_TIME = 1.0f;

		// ----------------------------------------------------
		// FLYING STATE CONSTANTS
		private const float DISAPPEARING_DISTANCE = 10.0f;
		private float DISAPPEARING_DISTANCE_SQR => DISAPPEARING_DISTANCE * DISAPPEARING_DISTANCE;



		// ---------------------------------------------------
		// STARTUP VARIABLES
		private float _startupTime = 0.0f;


		// ---------------------------------------------------
		// FLYING VARIABLES
		private float _flyingTime = 0.0f;


		private void StartingProcess(float delta)
		{
			_startupTime += delta;

			if (_startupTime >= STARTUP_TIME)
			{
				_curState = States.Flying;
				Scale = new Vector2(FINAL_SCALE, FINAL_SCALE);
				return;
			}
			
			float x = _startupTime / STARTUP_TIME;
			Position += _velocity * x * x;

			float newA = Mathf.Min(0.2f + x * x, 1.0f) * FINAL_SCALE;

			Scale = new(newA, newA);
		}

		private void FlyingProcess(float delta)
		{
			_flyingTime += delta;

			Vector2 diff = _targetPosition - GlobalPosition;

			if (diff.LengthSquared() < ENDING_DISTANCE_SQR || _flyingTime >= MAX_FLYING_TIME)
			{
				_curState = States.Ending;
				return;
			}

			_velocity += diff.Normalized() * LINEAR_ACCELERATION * (float)delta;
			_velocity = _velocity.LimitLength(MAX_VELOCITY);

			Position += _velocity;
			LookAt(_targetPosition);
		}

		private void EndingProcess(float delta)
		{
			Vector2 diff = _targetPosition - GlobalPosition;

			if (diff.LengthSquared() < DISAPPEARING_DISTANCE_SQR)
			{
				QueueFree();
			}

			_velocity += diff.Normalized() * LINEAR_ACCELERATION * 5 * (float) delta;
			_velocity = _velocity.LimitLength(MAX_VELOCITY);

			Position += _velocity;
			LookAt(_targetPosition);

			float newScale = Math.Min(1.0f, diff.LengthSquared() / DISAPPEARING_DISTANCE_SQR * 0.5f) * FINAL_SCALE;
			Scale = new(newScale, newScale);
		}
	}
}