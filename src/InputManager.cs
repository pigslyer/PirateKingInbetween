using Godot;

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

		private static readonly string[] ButtonToString = {
			"mv_right",
			"mv_up",
			"mv_left",
			"mv_down",
			"attack_melee",
			"attack_shoot",
			"player_carry",
		};

		private static string GetString(this InputButton button) => ButtonToString[(int) button];
	}

	public enum InputButton
	{
		MoveRight,
		MoveUp,
		MoveLeft,
		MoveDown,
		MeleeAttack,
		RangedAttack,
		Carry
	}
}