using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;
using PirateInBetween.Game.Player.Behaviours;

namespace PirateInBetween.Game.Player
{
    public class PlayerController : KinematicBody2D, IComboExecutor
	{
		private const int MAX_HEALTH = 100;
		private int _health = MAX_HEALTH;
		private Vector2 _velocity = Vector2.Zero;

#region Paths
        [Export] private NodePath __playerModelPath = null;

		[Export] private NodePath __playerBehaviourManagerPath = null;
		[Export] private NodePath __debugLabelPath = null;
		[Export] private NodePath __movingParentDetectorPath = null;
		[Export] private NodePath __cameraPath = null;
		[Export] private NodePath __uiManagerPath = null;
#endregion

		// Player's model.
        private PlayerModel _model;
		private PlayerBehaviourManager _behaviourManager;
		// A random label for putting shit on.
		private Label _debugLabel;
		private MovingParentDetector _detector;
		private CameraController _camera;
		private PlayerUIManager _uiManager;
		private PlayerCurrentFrameData _nextFrameData;

		public override void _Ready()
		{
			base._Ready();

            _model = GetNode<PlayerModel>(__playerModelPath);
			_behaviourManager = GetNode<PlayerBehaviourManager>(__playerBehaviourManagerPath);
			_debugLabel = GetNode<Label>(__debugLabelPath);
			_detector = GetNode<MovingParentDetector>(__movingParentDetectorPath);
			_camera = GetNode<CameraController>(__cameraPath);
			_uiManager = GetNode<PlayerUIManager>(__uiManagerPath);

			foreach (IDamageTaker taker in this.GetAllProgenyNodesOfType<IDamageTaker>())
			{
				taker.OnDamageTaken += OnDamageTakenSignalReceived;
			}

			_onDamageTaken = OnDamageTakenDefault();
			
			_behaviourManager.Initialize(this);
			_model.Initialize(this);

			_uiManager.UpdateHealth(_health, MAX_HEALTH);

			GenerateNextFrameData();
		}


		#region Controlling bit

		private Combo.OnHitReaction _onDamageTaken;

		private void OnDamageTakenSignalReceived(IDamageTaker takerSource, DamageDealer dealerSource, DamageData data)
		{
			DamageData appliedData = data.Apply(_onDamageTaken());

			if (appliedData.IsNull)
			{
				return;
			}

			if (appliedData.Damage > 0)
			{
				_health -= appliedData.Damage;
				_uiManager.UpdateHealth(_health, MAX_HEALTH);
			}

			if (_health <= 0)
			{
				DeathSequence();
			}

			bool stunned = appliedData.StunDuration > 0f;
			bool knocked = appliedData.Direction.LengthSquared() > 0f;

			Vector2 knockbackVector = appliedData.Direction * 200;

			if (stunned || knocked)
			{
				// i have no clue why i added this. dropping items?
			}

			if (stunned)
			{
				_behaviourManager.GetBehaviour<PlayerDamageHandler>(PlayerBehaviour.Behaviours.DamageHandler).StunFor(appliedData.StunDuration, _nextFrameData);
			}

			if (knocked)
			{
				_behaviourManager.GetBehaviour<PlayerDamageHandler>(PlayerBehaviour.Behaviours.DamageHandler).KnockbackFor(knockbackVector, _nextFrameData);
			}

		}

		private async Task DeathSequence()
		{
			const float deathTimer = 1f;

			_behaviourManager.StopActive();
			_behaviourManager.ActiveBehaviours = PlayerBehaviour.Behaviours.Dead;

			await this.WaitFor(deathTimer);

			_uiManager.ShowDeathMenu();
		}

		private bool _lastRight = true;

		public override void _PhysicsProcess(float delta)
		{
			_nextFrameData.Delta = delta;

			_movedCamera = _movingCamera;
			_movingCamera = false;

			if (_health <= 0)
			{
				_nextFrameData.CurrentAction = Animations.Dying;
			}

			RunBehaviours(_nextFrameData);


			if (_health <= 0 && _nextFrameData.Velocity.x != 0f)
			{
				_nextFrameData.FacingRight = _nextFrameData.Velocity.x < 0f;
			}

			ApplyFrameData(_nextFrameData);

			if (_movedCamera && !_movingCamera)
			{
				_camera.SetFollowing(true);
			}

			DebugOutOfBounds();
			DebugOutput();

			GenerateNextFrameData();
		}

		private void DebugOutput()
		{
			_debugLabel.Text = $"Fps: {Performance.GetMonitor(Performance.Monitor.TimeFps)}\nAnimation: {Enum.GetName(typeof(Animations), _nextFrameData.CurrentAction.Animation)}\nFacing right: {_nextFrameData.FacingRight}\nOn floor: {IsOnFloor()}\nBehaviours on floor: {_behaviourManager.IsPlayerOnFloor()}\nActive behaviours: {_behaviourManager.ActiveBehaviours}";
		}

		private void GenerateNextFrameData()
		{
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
				GD.PushError("Player action was never set this frame.");
				data.CurrentAction = Animations.Idle;
			}
			
			_lastRight = data.FacingRight;
			
			_model.UpdateModel(data);
		}

		private void DebugOutOfBounds()
		{
			if (GlobalPosition.y > 300f && _health > 0)
			{
				_health = -1;
				_uiManager.ShowDeathMenu();
			}
		}

		public MovingParentDetector GetMovingParentDetector() => _detector;

		#endregion


		#region IComboExecutor bit

		private bool _movingCamera = false;
		private bool _movedCamera = false;

		Vector2 IComboExecutor.CameraPosition
		{
			get => _camera.RelativePosition;
			set
			{
				_camera.SetFollowing(false);
				_movingCamera = true;
				_camera.RelativePosition = value;
			}
		}

		Vector2 IComboExecutor.GlobalPosition
		{
			get => GlobalPosition;
			set => MoveAndCollide(value - GlobalPosition);
		}

		void IComboExecutor.DealDamage(DamageDealerTargettingArea damageDealer, DamageData data) => _model.DamageDealerEnable(damageDealer, data);

		void IComboExecutor.StopDealingDamage(DamageDealerTargettingArea damageDealer) => _model.DamageDealerDisable(damageDealer);
		void IComboExecutor.TakeDamage(DamageTakerTargetArea to) => _model.DamageTakerEnable(to);
		void IComboExecutor.StopTakingDamage(DamageTakerTargetArea to) => _model.DamageTakerDisable(to);

		bool IComboExecutor.IsOnFloor 
		{ 
			get => _behaviourManager.IsPlayerOnFloor() && !_behaviourManager.GetBehaviour<PlayerJumping>(PlayerBehaviour.Behaviours.Jumping).HasJumped; 
		}

		void IComboExecutor.OnDamageTakenSet(Combo.OnHitReaction damageTaken) => _onDamageTaken = damageTaken;
		public Combo.OnHitReaction OnDamageTakenDefault() => _behaviourManager.GetBehaviour<PlayerDamageHandler>(PlayerBehaviour.Behaviours.DamageHandler).TakeDamage;
		#endregion
	}
}
