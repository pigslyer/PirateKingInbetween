using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	public interface ICombatFrameData
	{
		Vector2 Velocity { get; set; }
		float Delta { get; }
		bool FacingRight { get; }
	}
}