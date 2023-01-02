using Godot;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Universally useful extensions, no need for them to occupy any namespace. Partial to allow game specific additions.
/// </summary>
public static partial class Extensions
{

	public static float GetPercentDone(this Timer timer)
	{
		return 1f - timer.TimeLeft/timer.WaitTime;
	}


	/// <summary>
	/// Waits for alloted time.
	/// </summary>
	public static async Task WaitFor(this Node node, float time)
	{
		if (time > 0f)
		{
			await node.ToSignal(node.GetTree().CreateTimer(time), "timeout");
		}
	}

	/// <summary>
	/// Much like its functionless pair, but this one also checks the given condition. If given condition is ever false, timer preemptively stops
	/// </summary>
	public static async Task WaitFor(this Node node, float time, Func<bool> orFor)
	{
		if (time > 0f)
		{
			float t = 0f;
			while (t < time && !node.IsQueuedForDeletion() && orFor())
			{
				await node.ToSignal(node.GetTree(), "idle_frame");

				t += node.GetProcessDeltaTime();
			}
		}
	}
}
