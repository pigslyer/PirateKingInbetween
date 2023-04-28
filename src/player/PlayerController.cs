using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Pigslyer.PirateKingInbetween.Util.Behaviour;
using Pigslyer.PirateKingInbetween.Player.Behaviours;

namespace Pigslyer.PirateKingInbetween.Player
{
	public partial class PlayerController : CharacterBody2DOverride, IFrameData
	{
		[Export] public PlayerBehaviourProperties BehaviourProperties = null!;

		[ExportGroup("Camera settings")]
		[Export] private Camera2DController _cameraController = null!;
		[Export] private float _cameraOffset = 0.0f;

		[Export] private float _cameraBobStrength = 0.0f;
		[Export] private float _cameraBobSpeed = 0.0f;
		private Node2D _cameraFollowingNode = new();


		[Signal] public delegate void OnDamageTakenEventHandler();

		private BehaviourManager _behaviourManager = null!;
		private CharacterAnimation _lastAnimation = CharacterAnimation.None;

		public override void _Ready()
		{
			_behaviourManager = GenerateDefaultBehaviours();

			AddChild(_cameraFollowingNode);
			_cameraController.SetFollowNode(_cameraFollowingNode);

		}

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			_lastAnimation = CharacterAnimation.None;
			_behaviourManager.RunBehaviours();

			MoveAndSlide();

			ProcessCamera();
		}

		private void ProcessCamera()
		{
			float mult = Mathf.Sign(Velocity.X);

			_cameraFollowingNode.Position = Velocity.Normalized() * _cameraOffset;
			_cameraController.SetBob(mult * _cameraBobStrength, mult * _cameraBobSpeed);
		}

		float IFrameData.Delta => (float)GetPhysicsProcessDeltaTime();	
		bool IFrameData.IsOnFloor => IsOnFloor();
		CharacterAnimation IFrameData.CurrentAnimation
		{
			get => _lastAnimation;
			set
			{
				if (_lastAnimation == CharacterAnimation.None)
				{
					_lastAnimation = value;
				}
			}
		}

		private BehaviourManager GenerateDefaultBehaviours()
		{
			return BehaviourManager.Generate(
				new IBehaviour[]
				{
					new PlayerBehaviourMovement(this),
					new PlayerBehaviourModel(this),
				}
			);
		}
	}
}