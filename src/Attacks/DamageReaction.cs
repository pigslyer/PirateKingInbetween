using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageReaction
	{
		public static DamageReaction Zero => new DamageReaction(0f, 0f, 0f);
		public static DamageReaction One => new DamageReaction(1f, 1f, 1f);

		public readonly float KnockbackMult;
		public readonly float DamageMult;
		public readonly float StunMult;

		public DamageReaction(float knockbackMult, float damageMult, float stunMult)
		{
			KnockbackMult = knockbackMult; DamageMult = damageMult; StunMult = stunMult;
		}
	}
}