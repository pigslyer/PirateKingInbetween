using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageDealer : Area2D, IDamageTakerDetector
	{
		[Signal] public delegate void OnDamageDealt(DamageTaker obj);
		[Signal] public delegate void OnHit(Node what);

		[Export] public DamageDealerTargettingArea DealerType { get; private set; }

		private DamageData _currentDamageData = null;
		private DamageAmount? _currentDamageAmount;

		public override void _Ready()
		{
			base._Ready();

			Disable();

			Connect("area_entered", this, nameof(OnDamageDealtSignal));
			Connect("body_entered", this, nameof(OnHitSignal));
		}

		public async Task TempEnable(DamageData data, float time)
		{
			Enable(data);

			await this.WaitFor(time);

			Disable();
		}

		public void Enable(DamageData damageData)
		{
			_currentDamageData = damageData;

			DamageAllPresent();
		}

		protected void Enable(DamageAmount amount)
		{
			_currentDamageAmount = amount;
			_currentDamageData = null;
			
			DamageAllPresent();
		}

		/// <summary>
		/// In case something was detected already but the area wasn't properly enabled at the time.
		/// </summary>
		private void DamageAllPresent()
		{
			foreach (Godot.Area2D node in GetOverlappingAreas())
			{
				if (node is DamageTaker taker)
				{
					OnDamageDealtSignal(taker);
				}
			}
		}

		public void Disable()
		{
			_currentDamageAmount = null; _currentDamageData = null;
		}

		private void OnDamageDealtSignal(DamageTaker taker)
		{
			if (_currentDamageAmount == null && _currentDamageData == null)
			{
				return;
			}

			if (ShouldHit(taker))
			{
				taker.TakeDamage(GetDamageData(taker.GlobalPosition), this);
				EmitSignal(nameof(OnDamageDealt), taker);
			}
		}

		/// <summary>
		/// Represents the direction in which a damage amount passed with <see cref="DamageDealer.Enable(DamageAmount)"/> should be faced.
		/// If this method isn't overridden, <see cref="Vector2.Zero"/> is used, if <see cref="DamageDealer.Enable(DamageData)"/> was used, it is ignored.
		/// </summary>
		/// <returns></returns>
		protected virtual Vector2? GetDamageDirection(Vector2 targetGlobalPosition) => null;

		private DamageData GetDamageData(Vector2 position) => _currentDamageData ?? new DamageData((DamageAmount)_currentDamageAmount, GetDamageDirection(position));


		protected virtual bool ShouldHit(DamageTaker area) => true;

		private void OnHitSignal(Node node) => EmitSignal(nameof(OnHit), node);


		bool IDamageTakerDetector.CanSeeTaker() => GetOverlappingAreas().Count > 0;

		DamageTakerTargetArea IDamageTakerDetector.GetTakerArea() => ((DamageTaker)(GetOverlappingAreas()[0])).TakerType;
	}
}