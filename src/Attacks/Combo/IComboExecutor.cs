using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	// idk what this is yet, but could be useful
	public interface IComboExecutor
	{
		Vector2 GlobalPosition { get; set; }
		Vector2 CameraPosition { get; set; }
	}
}