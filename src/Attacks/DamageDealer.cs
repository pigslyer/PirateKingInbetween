using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageDealer : Area2D
	{
		[Signal] public delegate void OnDamageDealt(DamageTaker obj);
		[Signal] public delegate void OnHit(Node what);

		private DamageData _currentDamageData = null;
		private DamageAmount _currentDamageAmount;

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
			SetEnabled(true);
			_currentDamageData = damageData;
		}

		protected void Enable(DamageAmount amount)
		{
			SetEnabled(true);
			_currentDamageAmount = amount;
			_currentDamageData = null;
		}

		public void Disable() => SetEnabled(false);

		private void OnDamageDealtSignal(Area2D area)
		{
			if (area is DamageTaker taker && ShouldHit(taker))
			{
				taker.TakeDamage(GetDamageData(area.GlobalPosition));
				EmitSignal(nameof(OnDamageDealt), taker);
			}
			else
			{
				GD.PushWarning($"Collided with area which isn't {nameof(DamageTaker)}: {area.Name}");
			}
		}

		/// <summary>
		/// Represents the direction in which a damage amount passed with <see cref="DamageDealer.Enable(DamageAmount)"/> should be faced.
		/// If this method isn't overridden, <see cref="Vector2.Zero"/> is used, if <see cref="DamageDealer.Enable(DamageData)"/> was used, it is ignored.
		/// </summary>
		/// <returns></returns>
		protected virtual Vector2? GetDamageDirection(Vector2 targetGlobalPosition) => null;

		private DamageData GetDamageData(Vector2 position) => _currentDamageData ?? new DamageData(_currentDamageAmount, GetDamageDirection(position));

		protected virtual bool ShouldHit(DamageTaker area) => true;

		private void OnHitSignal(Node node) => EmitSignal(nameof(OnHit), node);
	}
}