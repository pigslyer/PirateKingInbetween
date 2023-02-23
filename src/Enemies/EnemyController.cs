using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;

namespace PirateInBetween.Game.Enemies
{
	public class EnemyController : KinematicBody2D, IComboExecutor
	{
		[Export] private int _maxHealth = 40;
		private int _health;
		private EnemyFrameData _nextFrameData;

		#region Paths
		[Export] private NodePath __enemyModelPath = null;
		[Export] private NodePath __basicBehaviourPath = null;
		[Export] private NodePath __damageTakerPath = null;
		#endregion

		private EnemyModel _model;
		private BasicBehaviour _behaviour;

		private Combo.OnHitReaction _defaultDamageReaction;


		public override void _Ready()
		{
			base._Ready();

			_model = GetNode<EnemyModel>(__enemyModelPath);
			_behaviour = GetNode<BasicBehaviour>(__basicBehaviourPath);
			GetNode<DamageTaker>(__damageTakerPath).OnDamageTaken += OnDamageTakenReaction;

			_behaviour.Initialize(this);
			_defaultDamageReaction = OnDamageTakenDefault();
			_health = _maxHealth;

			_nextFrameData = new EnemyFrameData()
			{
				Velocity = Vector2.Zero,
				FacingRight = true
			};
		}


		private void OnDamageTakenReaction(DamageTaker taker, DamageDealer dealer, DamageData data)
		{
			DamageData newData = data.Apply(_defaultDamageReaction());

			if (newData.Damage > 0)
			{
				GD.Print($"tooken {newData.Damage} damage");
				_health -= newData.Damage;
			}

			bool stunned = newData.StunDuration > 0f;
			bool knockedBack = newData.Direction.LengthSquared() > 0f;
			Vector2 knockbackVelocity = newData.Direction * newData.Damage * 20f;

			if (stunned)
			{
				GD.Print($"stunnen for {newData.StunDuration} time");
				_behaviour.OnStunned(newData.StunDuration, _nextFrameData);
			}

			if (knockedBack)
			{
				GD.Print($"knocken backen for {knockbackVelocity} velocity");
				_behaviour.OnKnockedback(knockbackVelocity, _nextFrameData);
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			_nextFrameData.Delta = delta;

			_behaviour.Run(_nextFrameData);
			ApplyFrameData(_nextFrameData);			

			_nextFrameData = new EnemyFrameData()
			{
				Velocity = _nextFrameData.Velocity,
				FacingRight = _nextFrameData.FacingRight
			};

		}

		private void ApplyFrameData(EnemyFrameData data)
		{
			_nextFrameData.Velocity = MoveAndSlide(_nextFrameData.Velocity, Vector2.Up);

			_model.Scale = new Vector2(data.FacingRight ? 1 : -1, 1);
		}

		#region IComboExecutor
		Vector2 IComboExecutor.CameraPosition { get; set; }

		bool IComboExecutor.IsOnFloor => IsOnFloor();

		public void DealDamage(ComboExecutorDamageDealers damageDealer, DamageData data) => _model.DealDamage(damageDealer, data);	

		public void StopDealingDamage(ComboExecutorDamageDealers damageDealer) => _model.StopDealingDamage(damageDealer);

		public void TakeDamage(ComboExecutorDamageTaker to) => _model.TakeDamage(to);

		public void StopTakingDamage(ComboExecutorDamageTaker to) => _model.StopTakingDamage(to);

		public void OnDamageTakenSet(Combo.OnHitReaction damageTaken) => _defaultDamageReaction = damageTaken;

		public Combo.OnHitReaction OnDamageTakenDefault() => _behaviour.DamageTakenReaction;

		#endregion
	}
}