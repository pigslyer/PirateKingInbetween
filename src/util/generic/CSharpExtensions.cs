using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public static class CSharpExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var val in enumerable)
			{
				action.Invoke(val);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
		{
			int i = 0;
			foreach (var val in enumerable)
			{
				action.Invoke(val, i++);
			}
		}
	}
}