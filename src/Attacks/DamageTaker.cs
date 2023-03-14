using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageTaker : Area2D, IDamageTaker
	{
		[Export] public DamageTakerTargetArea TakerType { get; private set; }

		public event EventDelegates.OnSomethingHappened<IDamageTaker, DamageDealer, DamageData> OnDamageTaken;

		public void TakeDamage(DamageData data, DamageDealer source) => OnDamageTaken(this, source, data);

		public void Enable() => SetEnabled(true);
		public void Disable() => SetEnabled(false);
	}
}