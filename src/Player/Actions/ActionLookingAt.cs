using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Actions
{
	public class ActionLookingAt : PlayerAction
	{
		public readonly string Description;

		public ActionLookingAt(PlayerAnimation animation, string description) : base(animation)
		{
			Description = description;
		}
	}
}