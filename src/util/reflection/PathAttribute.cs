using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Reflection;

namespace Pigslyer.PirateKingInbetween.Util.Reflection
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PathAttribute : Attribute
	{
		public readonly string Path;

		public PathAttribute(string path)
		{
			Path = path;
		}

		public static PackedScene LoadResource<T>() where T : class
		{
			Type type = typeof(T);
			PathAttribute? path = type.GetCustomAttribute<PathAttribute>();

			if (path == null)
			{
				throw new InvalidOperationException($"{type.ToString()} does not implement {nameof(PathAttribute)}.");
			}

			PackedScene ret = ResourceLoader.Load<PackedScene>(path.Path);

			if (ret == null)
			{
				throw new Exception($"Could not load {type.ToString()}.");
			}

			return ret;
		}
	}
}
