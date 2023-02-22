using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Represents a pair of int values which form a mathematical interval [Start, End], alongside helper methods for its use.
/// </summary>
public struct IntInterval : IEquatable<IntInterval>
{
	public static IntInterval All => new IntInterval(int.MinValue, int.MaxValue);

	public int Start;
	public int End;
	public int Delta => End - Start;

	public IntInterval(int start, int end)
	{
		Start = start; End = end;
	}

	public IntInterval(int end)
	{
		Start = 0; End = end;
	}


	public bool IsInRange(int what) => Start <= what && what <= End;

	public int GetRandom(RandomNumberGenerator generator = null)
	{
		return (int) (Start + (generator == null ? GD.Randi() : generator.Randi()) % Delta);
	}

	public override string ToString()
	{
		return $"[{Start}, {End}]";
	}


	public static implicit operator IntInterval(int end) => new IntInterval(end);
	public static implicit operator IntInterval((int start, int end) val) => new IntInterval(val.start, val.end);

	public static bool operator ==(IntInterval a, IntInterval b) => a.Start == b.Start && a.End == b.End;
	public static bool operator !=(IntInterval a, IntInterval b) => !(a == b);

	public bool Equals(IntInterval other) => this == other;

	public override bool Equals(object obj) => obj is IntInterval f ? f == this : false;

	public override int GetHashCode() => new Vector2(Start, End).GetHashCode();
}