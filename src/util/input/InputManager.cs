using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Input
{
	public static class InputManager
	{
		private static readonly IReadOnlyList<string> _actions;

		static InputManager()
		{
			List<string> actions = new();

			foreach (var actionName in Enum.GetNames(typeof(InputActions)))
			{
				if (InputMap.HasAction(actionName))
				{
					actions.Add(actionName);
				}
				else
				{
					Log.PushError(
						Log.Types.Input,
						$"Input {actionName} is not a registred {nameof(InputEventAction)}."
					);
				}
			}

			_actions = actions;
		}

		public static Vector2 GetMovementVectorI(
				InputActions posHor, InputActions negHor,
				InputActions posVert, InputActions negVert
			)
		{
			return new Vector2(
				GetDirectionDiff(posHor, negHor),
				GetDirectionDiff(posVert, negVert)
			);
		}

		public static float GetDirectionDiff(InputActions positiveDir, InputActions negativeDir)
		{
			return Godot.Input.GetActionStrength(_actions[(int)positiveDir]) - Godot.Input.GetActionStrength(_actions[(int)negativeDir]);
		}

		public static bool IsActionJustPressed(InputActions action, bool exactMatch = false) 
			=> Godot.Input.IsActionJustPressed(_actions[(int)action], exactMatch);

		public static bool IsActionJustReleased(InputActions action, bool exactMatch = false) 
			=> Godot.Input.IsActionJustReleased(_actions[(int)action], exactMatch);

		public static bool IsActionPressed(InputActions action, bool exactMatch = false) 
			=> Godot.Input.IsActionPressed(_actions[(int)action], exactMatch);
		
		public static string GetAction(this InputActions action) => _actions[(int)action];
	}
}