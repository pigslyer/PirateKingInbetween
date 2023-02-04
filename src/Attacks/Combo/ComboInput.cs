using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	/// <summary>
	/// Used to store both constant combo inputs when declaring them and for parsing player inputs.
	/// </summary>
	public class ComboInputContainer
	{
		private const float EPSILON = 0.3f;
		private static readonly float[] TIMINGS = ReflectionHelper.GetEnumFloats<ComboTiming>();

		public readonly bool IsConstant;

		public readonly ComboInput Input;
		private ComboTiming _timing;
		private float _time;

		public ComboInputContainer(ComboInput input, ComboTiming timing)
		{
			IsConstant = true;
			Input = input;
			_timing = timing;
		}

		public static implicit operator ComboInputContainer(ComboInput input) => new ComboInputContainer(input, ComboTiming.Short);

		public ComboInputContainer(ComboInput input, float time)
		{
			IsConstant = false;
			Input = input;
			_time = time;
		}

		public bool EqualsInput(ComboInputContainer o) => Input == o.Input;

		public static bool operator ==(ComboInputContainer a, ComboInputContainer b)
		{
			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
			{
				return ReferenceEquals(a, b);
			}

			if (a.IsConstant == b.IsConstant)
			{
				return a.Input == b.Input && a._timing == b._timing && a._time == b._time;
			}
			
			if (a.IsConstant)
			{
				var temp = a;
				a = b;
				b = temp;
			}

			return a.Input == b.Input && a._time - TIMINGS[(int) b._timing] < EPSILON;
		}

		public static bool operator !=(ComboInputContainer a, ComboInputContainer b) => !(a == b);

		public override string ToString()
		{
			if (IsConstant) 
			{
				return $"Constant {nameof(ComboInput)}, Input: {Input}, Timing: {_timing}";
			}

			var a = Array.FindIndex(TIMINGS, t => _time - t < EPSILON);
			return $"Dynamic {nameof(ComboInput)}, Input: {Input}, Time: {_time}, Timing: {(a == -1 ? "None" : Enum.GetName(typeof(ComboTiming), a))}";
		}
		public override bool Equals(object obj)
		{
			if (obj is ComboInput input)
			{
				return this == input;
			}

			return false;
		}

		public override int GetHashCode() => (IsConstant ? 1 : 2) * 103 + (int) Input * 31 + (int) _timing * 83 + (int) (_time * 23);
	}

	public enum ComboInput
	{
		Forward,
		SwitchDirection,
		Up,
		Down,
		Melee,
		Ranged,
	};

	public enum ComboTiming
	{
		[EnumFloat(0.2f)] Short,
		[EnumFloat(0.4f)] Medium,
		[EnumFloat(0.6f)]Long,
	};
}