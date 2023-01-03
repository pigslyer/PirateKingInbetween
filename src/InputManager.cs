using Godot;

namespace PirateInBetween
{
	public static class InputManager
	{
		
		public static Vector2 GetMovementVector() => Input.GetVector(
			Button.MoveLeft.GetString(), 
			Button.MoveRight.GetString(), 
			Button.MoveUp.GetString(), 
			Button.MoveDown.GetString()
		);

		public static bool IsActionPresseed(Button button) => Input.IsActionPressed(button.GetString());
		public static bool IsActionJustPressed(Button button) => Input.IsActionJustPressed(button.GetString());
		public static bool IsActionJustReleased(Button button) => Input.IsActionJustReleased(button.GetString());

		private static readonly string[] ButtonToString = {
			"mv_right",
			"mv_up",
			"mv_left",
			"mv_down",
			"attack_melee",
			"attack_shoot",
		};

		private static string GetString(this Button button) => ButtonToString[(int) button];
	}

	public enum Button
	{
		MoveRight,
		MoveUp,
		MoveLeft,
		MoveDown,
		MeleeAttack,
		RangedAttack,
	}
}