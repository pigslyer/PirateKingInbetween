using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageTaker : Area2D
	{

		public delegate void OnDamageTakenEventHandler(DamageTaker source, DamageData data);
		public event OnDamageTakenEventHandler OnDamageTaken;

		public void TakeDamage(DamageData data) => OnDamageTaken(this, data);

		public void Enable() => SetEnabled(true);
		public void Disable() => SetEnabled(false);
	}
}