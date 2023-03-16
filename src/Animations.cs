using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public enum Animations
	{
		[EnumString("Idle")] Idle,
		[EnumString("Jump")] Jump,
		[EnumString("Fall")] Fall,
		[EnumString("Run")] Run,
		[EnumString("Dying")] Dying,
		[EnumString("Pushing")] Pushing,
		[EnumString("Stunned")] Stunned,
		[EnumString("StunnedMoving")] StunnedMoving,
		[EnumString("StunnedAir")] StunnedAir,
		[EnumString("Idle")] WIP,
		[EnumString("BasicCombo1")] BasicCombo1,
		[EnumString("BasicCombo2")] BasicCombo2,
		[EnumString("BasicCombo3")] BasicCombo3,
		[EnumString("BasicShoot1")] BasicShoot1,
		[EnumString("DodgeSlash")] DodgeSlash,
		[EnumString("JumpSlashFromGround")] Uppercut,
		[EnumString("JumpSlash")] JumpAttack,
	}

	public static class AnimationsExtensions
	{
		private static string[] _animationStrings = ReflectionHelper.GetEnumStrings<Animations>();

		public static string GetString(this Animations animation) => _animationStrings[(int) animation];
	}
}