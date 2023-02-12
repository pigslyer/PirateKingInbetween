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
		public int AmountOfDamage;
		public float Stun;

		public DamageAmount(int amount, float stun)
		{
			AmountOfDamage = amount; Stun = stun;
		}
		public static implicit operator DamageAmount(int amount) => new DamageAmount(amount, 0f);

		public static implicit operator int(DamageAmount data) => data.AmountOfDamage;
	}

	public class DamageData
	{
		public int Damage => DamageAmount.AmountOfDamage;
		public float StunDuration => DamageAmount.Stun;
		
		public readonly DamageAmount DamageAmount;
		public readonly Vector2 Direction;

		public DamageData(DamageAmount damage, Vector2? direction = null)
		{
			DamageAmount = damage; Direction = direction ?? Vector2.Zero;
		}

		public DamageData Apply(DamageReaction reaction) => new DamageData(
			new DamageAmount(
				Mathf.RoundToInt(
					DamageAmount.AmountOfDamage * reaction.DamageMult)
					, DamageAmount.Stun * reaction.StunMult
				),
				Direction * reaction.KnockbackMult
			);
	}
}