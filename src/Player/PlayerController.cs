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
		[Export] private NodePath __damageTakerPath = null;
#endregion

		// Player's model.
        private PlayerModel _model;
		private PlayerBehaviourManager _behaviourManager;
		// A random label for putting shit on.
		private Label _debugLabel;
		private MovingParentDetector _detector;
		private CameraController _camera;
		private DamageTaker _damageTaker;

		private PlayerCurrentFrameData _nextFrameData;

		public override void _Ready()
		{
			base._Ready();

            _model = GetNode<PlayerModel>(__playerModelPath);
			_behaviourManager = GetNode<PlayerBehaviourManager>(__playerBehaviourManagerPath);
			_debugLabel = GetNode<Label>(__debugLabelPath);
			_detector = GetNode<MovingParentDetector>(__movingParentDetectorPath);
			_camera = GetNode<CameraController>(__cameraPath);
			_damageTaker = GetNode<DamageTaker>(__damageTakerPath);

			_behaviourManager.Initialize(this);

			OnDamageTakenReset();
			_damageTaker.Connect(nameof(DamageTaker.OnDamageTaken), this, nameof(OnDamageTakenSignalReceived));

			_nextFrameData = new PlayerCurrentFrameData()
			{
				Velocity = _velocity,
				VelocityMult = 1f,
				FacingRight = _lastRight,
			};
		}


		#region Controlling bit

		private Action<PlayerCurrentFrameData, DamageData> _onDamageTaken;

		private void OnDamageTakenSignalReceived(DamageData data) => _onDamageTaken(_nextFrameData, data);

		private void OnDamageTakenBase(PlayerCurrentFrameData frameData, DamageData data)
		{
			frameData.Velocity = new Vector2(1000,-1000);
		}

		public void OnDamageTakenReset() => _onDamageTaken = OnDamageTakenBase;

		private bool _lastRight = true;

		public override void _PhysicsProcess(float delta)
		{
			_nextFrameData.Delta = delta;

			_movedCamera = _movingCamera;
			_movingCamera = false;

			RunBehaviours(_nextFrameData);

			//GD.Print($"velocity: {_nextFrameData.Velocity}");

			ApplyFrameData(_nextFrameData);

			if (_movedCamera && !_movingCamera)
			{
				_camera.SetFollowing(true);
			}
			
			DebugOutOfBounds();

			_debugLabel.Text = $"Animation: {Enum.GetName(typeof(PlayerAnimation), _nextFrameData.CurrentAction.Animation)}\nFacing right: {_nextFrameData.FacingRight}\nOn floor: {IsOnFloor()}\nBehaviours on floor: {_behaviourManager.IsPlayerOnFloor()}\nActive behaviours: {_behaviourManager.ActiveBehaviours}";

			_nextFrameData = new PlayerCurrentFrameData()
			{
				Velocity = _velocity,
				VelocityMult = 1f,
				FacingRight = _lastRight,
			};
		}

		private void RunBehaviours(PlayerCurrentFrameData data) => _behaviourManager.RunBehaviours(data);

		private void ApplyFrameData(PlayerCurrentFrameData data)
		{
			if (data.VelocityMult > 0f && data.Velocity.LengthSquared() > 0f)
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

		#endregion


		#region IComboExecutor bit

		private bool _movingCamera = false;
		private bool _movedCamera = false;

		Vector2 IComboExecutor.CameraPosition
		{
			get => _camera.GlobalPosition - GlobalPosition;
			set
			{
				_camera.SetFollowing(false);
				_movingCamera = true;
				_camera.GlobalPosition = GlobalPosition + value;
			}
		}

		Vector2 IComboExecutor.GlobalPosition
		{
			get => GlobalPosition;
			set => MoveAndCollide(value - GlobalPosition);
		}

		void IComboExecutor.DealDamage(ComboExecutorDamageDealers damageDealer, DamageAmount data) => _model.DamageDealerEnable(damageDealer, data);

		void IComboExecutor.StopDealingDamage(ComboExecutorDamageDealers damageDealer) => _model.DamageDealerDisable(damageDealer);
		void IComboExecutor.TakeDamage(ComboExecutorDamageTaker to) => _model.DamageTakerEnable(to);
		void IComboExecutor.StopTakingDamage(ComboExecutorDamageTaker to) => _model.DamageTakerDisable(to);

		bool IComboExecutor.IsOnFloor { get => _behaviourManager.IsPlayerOnFloor(); }

		void IComboExecutor.OnDamageTakenSet(Action<ICombatFrameData, DamageData> damageTaken) => _onDamageTaken = damageTaken;
		#endregion
	}
}
