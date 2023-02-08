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

		private DamageData _currentDamage = null;

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

		public void Enable(DamageData data)
		{
			SetEnabled(true);
			_currentDamage = data;
		}

		public void Disable() => SetEnabled(false);

		private void OnDamageDealtSignal(Area2D area)
		{
			if (area is DamageTaker taker && ShouldHit(taker))
			{
				taker.TakeDamage(_currentDamage);
				EmitSignal(nameof(OnDamageDealt), taker);
			}
			else
			{
				GD.PushWarning($"Collided with area which isn't {nameof(DamageTaker)}: {area.Name}");
			}
		}

		protected virtual bool ShouldHit(DamageTaker area) => true;

		private void OnHitSignal(Node node) => EmitSignal(nameof(OnHit), node);
	}
}