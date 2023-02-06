using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Represents a pair of float values which form a mathematical interval [Start, End], alongside helper methods for its use.
/// </summary>
public struct FloatInterval : IEquatable<FloatInterval>
{
	public static FloatInterval All => new FloatInterval(float.NegativeInfinity, float.PositiveInfinity);

	public float Start;
	public float End;
	public float Delta => End - Start;

	public FloatInterval(float start, float end)
	{
		Start = start; End = end;
	}

	public FloatInterval(float end)
	{
		Start = 0f; End = end;
	}

	public static implicit operator FloatInterval(float end) => new FloatInterval(end);
	public static implicit operator FloatInterval((float start, float end) val) => new FloatInterval(val.start, val.end);

	public bool IsInRange(float what) => Start <= what && what <= End;

	public override string ToString()
	{
		return $"[{Start}, {End}]";
	}

	public static bool operator==(FloatInterval a, FloatInterval b) => a.Start == b.Start && a.End == b.End;
	public static bool operator!=(FloatInterval a, FloatInterval b) => !(a == b);

	public bool Equals(FloatInterval other) => this == other;

	public override bool Equals(object obj) => obj is FloatInterval f ? f == this : false;

	public override int GetHashCode() => new Vector2(Start, End).GetHashCode();
}