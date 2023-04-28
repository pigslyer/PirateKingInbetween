using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public partial class Ref<T> : RefCounted
	{
		public readonly T Value;

		public Ref(T value)
		{
			Value = value;
		}

		public static implicit operator Ref<T>(T val) => new(val);
		public static implicit operator T(Ref<T> val) => val.Value;
	}
}