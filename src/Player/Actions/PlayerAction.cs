using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Actions
{
	public class PlayerAction
	{
		public readonly Animations Animation;
		public readonly float? PercentageDone = null;

		public PlayerAction(Animations animation)
		{
			Animation = animation;
		}

		public PlayerAction(Combos.CombatAnimation animation)
		{
			Animation = animation.PlayingAnimation;
			PercentageDone = animation.PercentangeDone;
		}

		public static implicit operator PlayerAction(Animations animation) => new PlayerAction(animation);
	}
}