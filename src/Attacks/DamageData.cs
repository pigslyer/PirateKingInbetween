using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public struct DamageAmount
	{
		public int Amount;

		public DamageAmount(int amount)
		{
			Amount = amount;
		}
		public static implicit operator DamageAmount(int amount) => new DamageAmount(amount);
	}

	public class DamageData : Reference
	{
		public readonly DamageAmount Damage;
		public readonly Func<Vector2> Source;

		public DamageData(DamageAmount damage, Func<Vector2> source)
		{
			Damage = damage; Source = source;
		}
	}
}