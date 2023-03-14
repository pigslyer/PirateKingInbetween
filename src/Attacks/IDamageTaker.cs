using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public interface IDamageTaker
	{
		DamageTakerTargetArea TakerType { get; }

		event EventDelegates.OnSomethingHappened<IDamageTaker, DamageDealer, DamageData> OnDamageTaken;

		void TakeDamage(DamageData data, DamageDealer source);
		void Enable();
		void Disable();
	}
}