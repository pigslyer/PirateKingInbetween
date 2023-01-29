using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;


namespace PirateInBetween
{
	public static class InputManager
	{
		/// <summary>
		/// Maps int values of <see cref="InputButton"/> to their <see cref="EnumString"/> string value.
		/// </summary>
		private static readonly string[] _enumToString = Extensions.GetEnumStrings<InputButton>();

		public static Vector2 GetMovementVector()
		{
			// HTML5 export seems to have trouble with GetVector
			if (OS.HasFeature("HTML5"))
				return new Vector2(
					Input.GetActionStrength(InputButton.MoveRight.GetString()) - Input.GetActionStrength(InputButton.MoveLeft.GetString()),
					Input.GetActionStrength(InputButton.MoveDown.GetString()) - Input.GetActionStrength(InputButton.MoveUp.GetString())	
				).Normalized();
				
			return Input.GetVector(
				InputButton.MoveLeft.GetString(), InputButton.MoveRight.GetString(), 
				InputButton.MoveUp.GetString(), InputButton.MoveDown.GetString()
			);
		
		}

		public static bool IsActionPresseed(InputButton button) => Input.IsActionPressed(button.GetString());
		public static bool IsActionJustPressed(InputButton button) => Input.IsActionJustPressed(button.GetString());
		public static bool IsActionJustReleased(InputButton button) => Input.IsActionJustReleased(button.GetString());

		private static string GetString(this InputButton button) => _enumToString[(int) button];
	}

	public enum InputButton
	{
		[EnumString("mv_right")] 			MoveRight,
		[EnumString("mv_up")] 				MoveUp,
		[EnumString("mv_left")] 			MoveLeft,
		[EnumString("mv_down")] 			MoveDown,
		[EnumString("attack_melee")] 		MeleeAttack,
		[EnumString("attack_shoot")]		RangedAttack,
		[EnumString("player_carry")] 		Carry,
		[EnumString("interact")]			Interact,
	}
}