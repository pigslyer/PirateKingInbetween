using System;
using Godot;

namespace PirateInBetween
{
	public static class Extensions
	{

		public static float GetPercentDone(this Timer timer)
		{
			return 1f - timer.TimeLeft/timer.WaitTime;
		}


	}
}