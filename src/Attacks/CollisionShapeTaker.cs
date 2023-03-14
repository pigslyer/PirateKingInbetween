using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class CollisionShapeTaker : CollisionShape2D, IDamageTaker
	{
		[Export] private DamageTakerTargetArea _takerType;
		public event EventDelegates.OnSomethingHappened<IDamageTaker, DamageDealer, DamageData> OnDamageTaken;

		DamageTakerTargetArea IDamageTaker.TakerType { get => _takerType; }


		public void Disable() => Disabled = true;

		public void Enable() => Disabled = false;

		public void TakeDamage(DamageData data, DamageDealer source) => OnDamageTaken(this, source, data);
	}
}