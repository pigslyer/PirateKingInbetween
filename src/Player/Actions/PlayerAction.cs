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
		public readonly PlayerAnimation Animation;

		public PlayerAction(PlayerAnimation animation)
		{
			Animation = animation;
		}

		public static implicit operator PlayerAction(PlayerAnimation animation) => new PlayerAction(animation);
	}
}