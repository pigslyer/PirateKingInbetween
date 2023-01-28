using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageData : Reference
	{
		public readonly int Damage;
		public readonly Func<Vector2> Source;

		public DamageData(int damage, Func<Vector2> source)
		{
			Damage = damage; Source = source;
		}
	}
}