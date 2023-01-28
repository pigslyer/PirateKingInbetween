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
		/// Maps int values of <see cref="InputButton"/> to their <see cref="InputButtonString"/> string value.
		/// </summary>
		private static readonly string[] _enumToString;

		static InputManager()
		{
			List<string> temp = new List<string>();

			foreach (var e in Enum.GetValues(typeof(InputButton)))
			{
				temp.Add(typeof(InputButton).GetField(Enum.GetName(typeof(InputButton), e)).GetCustomAttribute<InputButtonString>().Button);
			}

			_enumToString = temp.ToArray();
		} 		


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

		//private static string GetString(this InputButton button) => button.GetType().GetField(button.ToString()).GetCustomAttribute<InputButtonString>().Button;
		private static string GetString(this InputButton button) => _enumToString[(int) button];

		[AttributeUsage(AttributeTargets.Field)]
		public class InputButtonString : Attribute
		{
			public string Button { get; private set; }

			public InputButtonString(string button)
			{
				Button = button;
			}
		}
	}

	public enum InputButton
	{
		[InputManager.InputButtonString("mv_right")] 			MoveRight,
		[InputManager.InputButtonString("mv_up")] 				MoveUp,
		[InputManager.InputButtonString("mv_left")] 			MoveLeft,
		[InputManager.InputButtonString("mv_down")] 			MoveDown,
		[InputManager.InputButtonString("attack_melee")] 		MeleeAttack,
		[InputManager.InputButtonString("attack_shoot")]		RangedAttack,
		[InputManager.InputButtonString("player_carry")] 		Carry,
		[InputManager.InputButtonString("interact")]			Interact,
	}
}