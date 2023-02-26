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
		public readonly string Text;

		public ActionLookingAt(Animations animation, string description) : base(animation)
		{
			Text = description;
		}
	}
}