using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public static class GodotExtensions
	{
		public static bool IsActionPressed(this InputEvent ev, InputActions action, bool allowEcho = false, bool exactMatch = false) 
			=> ev.IsActionPressed(action.GetAction(), allowEcho, exactMatch);

		public static bool IsActionReleased(this InputEvent ev, InputActions action, bool exactMatch = false)
			=> ev.IsActionReleased(action.GetAction(), exactMatch);

		public static bool IsAction(this InputEvent ev, InputActions action, bool exactMatch = false)
			=> ev.IsAction(action.GetAction(), exactMatch);

	}
}