using System;
using Godot;

using System.Reflection;

namespace PirateInBetween
{
	public static class InputManager
	{
		
		public static Vector2 GetMovementVector()
		{
			if (OS.HasFeature("HTML5"))
				return new Vector2(
					Input.GetActionStrength(InputButton.MoveRight.GetString()) - Input.GetActionStrength(InputButton.MoveLeft.GetString()),
					Input.GetActionStrength(InputButton.MoveDown.GetString()) - Input.GetActionStrength(InputButton.MoveUp.GetString())	
				);
				
			return Input.GetVector(
				InputButton.MoveLeft.GetString(), InputButton.MoveRight.GetString(), 
				InputButton.MoveUp.GetString(), InputButton.MoveDown.GetString()
			);
		
		}

		public static bool IsActionPresseed(InputButton button) => Input.IsActionPressed(button.GetString());
		public static bool IsActionJustPressed(InputButton button) => Input.IsActionJustPressed(button.GetString());
		public static bool IsActionJustReleased(InputButton button) => Input.IsActionJustReleased(button.GetString());

		/*
		private static readonly string[] ButtonToString = {
			"mv_right", // MoveRight
			"mv_up", // MoveUp
			"mv_left", // MoveLeft
			"mv_down", // MoveDown
			"attack_melee", // MeleeAttack
			"attack_shoot", // RangedAttack
			"player_carry",
		};
		*/

		private static string GetString(this InputButton button) => button.GetType().GetField(button.ToString()).GetCustomAttribute<InputButtonString>().Button;

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
	}
}