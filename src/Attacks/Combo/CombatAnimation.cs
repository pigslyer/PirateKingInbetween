using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	public struct CombatAnimation
	{
		public readonly Animations PlayingAnimation;
		public readonly float? PercentangeDone;

		public CombatAnimation(Animations animation, float? percentageDone = null)
		{
			PlayingAnimation = animation; PercentangeDone = percentageDone;
		}

		public CombatAnimation(Animations animation, float currentTime, float targetLength)
		{ 
			PlayingAnimation = animation; PercentangeDone = currentTime / targetLength;
		}
	}
}