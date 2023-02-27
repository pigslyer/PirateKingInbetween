using Godot;
using System;

namespace PirateInBetween.Game.Autoloads
{
	public class Global : Autoload<Global>
	{
		public static bool PlayerHasWoodenLeg;


		public override void _Input(InputEvent ev)
		{
			base._Input(ev);

			if (ev.IsActionPressed("temp_wood_leg"))
			{
				PlayerHasWoodenLeg = !PlayerHasWoodenLeg;
			}
		}
	}
}

