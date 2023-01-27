using System.Diagnostics;
using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player
{
    public class PlayerController : KinematicBody2DOverride
	{
		private Vector2 _velocity = Vector2.Zero;

#region Paths
        [Export] private NodePath _playerModelPath = null;

		[Export] private NodePath _playerBehaviourManagerPath = null;
		[Export] private NodePath _debugLabelPath = null;
		[Export] private NodePath _movingParentDetectorPath = null;
#endregion

		// Player's model.
        private PlayerModel _model;
		private PlayerBehaviourManager _behaviourManager;
		// A random label for putting shit on.
		private Label _debugLabel;
		private MovingParentDetector _detector;

		public override void _Ready()
		{
			base._Ready();

            _model = GetNode<PlayerModel>(_playerModelPath);
			_behaviourManager = GetNode<PlayerBehaviourManager>(_playerBehaviourManagerPath);
			_debugLabel = GetNode<Label>(_debugLabelPath);
			_detector = GetNode<MovingParentDetector>(_movingParentDetectorPath);

			_behaviourManager.Initialize(this);
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

			RunBehaviours(data);
			ApplyFrameData(data);

			DebugOutOfBounds();

			_debugLabel.Text = $"Animation: {Enum.GetName(typeof(PlayerAnimation), data.NextAnimation)}\nFacing right: {data.FacingRight}\nOn floor: {IsOnFloor()}\nBehaviours on floor: {_behaviourManager.IsPlayerOnFloor()}";
		}

		private void RunBehaviours(PlayerCurrentFrameData data) => _behaviourManager.RunBehaviours(data);

		private void ApplyFrameData(PlayerCurrentFrameData data)
		{
			if (data.VelocityMult > 0f)
			{
				_velocity = MoveAndSlide(data.Velocity * data.VelocityMult, Vector2.Up) / data.VelocityMult;
			}

			if (data.NextAnimation == null)
			{
				PushError("Player animation was never set this frame.");
				data.NextAnimation = PlayerAnimation.Idle;
			}

			_model.SetAnimation((PlayerAnimation) data.NextAnimation, data.FacingRight);
			
			_lastRight = data.FacingRight;

			if (data.AttackData != null)
			{
				if (data.AttackData is SlashData slash)
				{
					_model.PlaySlash(slash);
				}
				else if (data.AttackData is ProjectileData bullet)
				{
					_model.Shoot(bullet);
				}
			}
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
