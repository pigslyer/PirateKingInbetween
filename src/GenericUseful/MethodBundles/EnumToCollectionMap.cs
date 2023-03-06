using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class EnumToCollectionMap<T, E> where E : Enum
{
	private readonly IList<ICollection<T>> _map;

	public EnumToCollectionMap(IEnumerable<T> from, Func<T, E> determineMapping)
	{
		_map = new ICollection<T>[Enum.GetNames(typeof(E)).Length];

		for (int i = 0; i < _map.Count; i++)
		{
			_map[i] = new LinkedList<T>();
		}

		foreach (T element in from)
		{
			_map[(int)(object)determineMapping(element)].Add(element);
		}
	}

	public void DoForAll(Action<T> what)
	{
		foreach (var coll in _map)
		{
			foreach (var element in coll)
			{
				what(element);
			}
		}
	}

	public void DoFor(Action<T> what, E type)
	{
		foreach (var element in _map[(int)(object) type])
		{
			what(element);
		}
	}
}