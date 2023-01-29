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
		[Signal] public delegate void OnDamageTaken(DamageData data);

		public void TakeDamage(DamageData data) => EmitSignal(nameof(OnDamageTaken), data);
	}
}