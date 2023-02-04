using System.Diagnostics;
using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;
using PirateInBetween.Game.Player.Behaviours;

namespace PirateInBetween.Game.Player
{
    public class PlayerController : KinematicBody2D, IComboExecutor
	{
		private Vector2 _velocity = Vector2.Zero;

#region Paths
        [Export] private NodePath __playerModelPath = null;

		[Export] private NodePath __playerBehaviourManagerPath = null;
		[Export] private NodePath __debugLabelPath = null;
		[Export] private NodePath __movingParentDetectorPath = null;
		[Export] private NodePath __cameraPath = null;
#endregion

		// Player's model.
        private PlayerModel _model;
		private PlayerBehaviourManager _behaviourManager;
		// A random label for putting shit on.
		private Label _debugLabel;
		private MovingParentDetector _detector;
		private CameraController _camera;

		public override void _Ready()
		{
			base._Ready();

            _model = GetNode<PlayerModel>(__playerModelPath);
			_behaviourManager = GetNode<PlayerBehaviourManager>(__playerBehaviourManagerPath);
			_debugLabel = GetNode<Label>(__debugLabelPath);
			_detector = GetNode<MovingParentDetector>(__movingParentDetectorPath);
			_camera = GetNode<CameraController>(__cameraPath);

			_behaviourManager.Initialize(this);

		}

		private bool _movingCamera = false;
		private bool _movedCamera = false;

		Vector2 IComboExecutor.CameraPosition
		{
			get => _camera.GlobalPosition;
			set
			{
				_camera.SetFollowing(false);
				_movingCamera = true;
				_camera.GlobalPosition = value;
			}
		}

		Vector2 IComboExecutor.GlobalPosition
		{
			get => GlobalPosition;
			set => MoveAndCollide(value - GlobalPosition);
		}

		private bool _lastRight = true;

		public override void _PhysicsProcess(float delta)
		{
			var data = new PlayerCurrentFrameData(delta)
			{
				Velocity = _velocity,
				VelocityMult = 1f,
				FacingRight = _lastRight,
			};

			_movedCamera = _movingCamera;
			_movingCamera = false;

			RunBehaviours(data);
			ApplyFrameData(data);

			if (_movedCamera && !_movingCamera)
			{
				_camera.SetFollowing(true);
			}
			
			DebugOutOfBounds();

			_debugLabel.Text = $"Animation: {Enum.GetName(typeof(PlayerAnimation), data.CurrentAction.Animation)}\nFacing right: {data.FacingRight}\nOn floor: {IsOnFloor()}\nBehaviours on floor: {_behaviourManager.IsPlayerOnFloor()}\nActive behaviours: {_behaviourManager.ActiveBehaviours}";
		}

		private void RunBehaviours(PlayerCurrentFrameData data) => _behaviourManager.RunBehaviours(data);

		private void ApplyFrameData(PlayerCurrentFrameData data)
		{
			if (data.VelocityMult > 0f)
			{
				_velocity = MoveAndSlide(data.Velocity * data.VelocityMult, Vector2.Up) / data.VelocityMult;
			}

			if (data.CurrentAction == null)
			{
				PushError("Player action was never set this frame.");
				data.CurrentAction = PlayerAnimation.Idle;
			}
			
			_lastRight = data.FacingRight;
			
			_model.SetAnimation(data.CurrentAction, data.FacingRight);
		}

		private void DebugOutOfBounds()
		{
			if (GlobalPosition.y > 300f)
			{
				GetTree().ReloadCurrentScene();
			}
		}

		public MovingParentDetector GetMovingParentDetector() => _detector;
		
	}
}
