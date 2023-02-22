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
		[EnumString("Pushing")] Pushing,
		[EnumString("Stunned")] Stunned,
		[EnumString("StunnedMoving")] StunnedMoving,
		[EnumString("StunnedAir")] StunnedAir,
	}
}