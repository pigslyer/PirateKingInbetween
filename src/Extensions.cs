using Godot;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

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

	public static void Reparent(this Node child, Node newParent)
	{
		child.GetParent().RemoveChild(child);
		newParent.AddChild(child);
	}

	public static void Reparent2D(this Node2D child, Node newParent)
	{
		Vector2 temp = child.GlobalPosition;
		Reparent(child, newParent);
		child.GlobalPosition = temp;
	}

	public static Godot.Area2D[] GetAreas(this Godot.Area2D area) => area.GetOverlappingAreas().Cast<Godot.Area2D>().ToArray();

	public static T Min<T>(this IEnumerable<T> enumerable, Func<T, int> standard) where T : Node2D
	{
		IEnumerator<T> e = enumerable.GetEnumerator();
		e.Reset();
		
		if (!e.MoveNext()) return null;
		T ret = e.Current;
		int val = standard(ret);
		int temp;

		while (e.MoveNext())
		{
			temp = standard(e.Current);

			if (temp < val)
			{
				val = temp;
				ret = e.Current;
			}
		}

		return ret;
	}

	/// <summary>
	/// For use with incrementing enums which use <see cref="EnumString"/>.
	/// </summary>
	/// <typeparam name="T">An auto-incrementing enum, each entry of which has the attribute <see cref="EnumString"/>.</typeparam>
	/// <returns>An array of values <see cref="EnumString.String"/> mapped to their index in <see cref="T"/>.</returns>
	public static string[] GetEnumStrings<T>() where T : Enum
	{
		Type t = typeof(T);
		List<string> temp = new List<string>();

		foreach (var e in Enum.GetValues(t))
		{
			temp.Add(t.GetField(Enum.GetName(t, e)).GetCustomAttribute<EnumString>().String);
		}

		return temp.ToArray();
	}


}

[AttributeUsage(AttributeTargets.Field)]
public class EnumString : Attribute
{
	public readonly string String;

	public EnumString(string @string)
	{
		String = @string;
	}
}