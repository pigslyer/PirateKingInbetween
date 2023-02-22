using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Reflection;

public static class ReflectionHelper
{

	/// <summary>
	/// For use with incrementing enums which use attribute A.
	/// </summary>
	/// <typeparam name="E"></typeparam>
	/// <typeparam name="A"></typeparam>
	/// <returns></returns>
	public static A[] GetEnumsAttribute<E, A>() where E : Enum where A : Attribute
	{
		Type t = typeof(E);
		List<A> temp = new List<A>();

		foreach (var e in Enum.GetValues(t))
		{

			temp.Add(t.GetField(Enum.GetName(t, e)).GetCustomAttribute<A>());
		}

		return temp.ToArray();
	}

	
	/// <summary>
	/// For use with incrementing enums which use <see cref="EnumString"/>.
	/// </summary>
	/// <typeparam name="T">An auto-incrementing enum, each entry of which has the attribute <see cref="EnumString"/>.</typeparam>
	/// <returns>An array of values <see cref="EnumString.String"/> mapped to their index in T.</returns>
	public static string[] GetEnumStrings<T>() where T : Enum => GetEnumsAttribute<T, EnumString>().Select(e => e?.String).ToArray();
	public static float?[] GetEnumFloats<T>() where T : Enum => GetEnumsAttribute<T, EnumFloat>().Select(e => e?.Float).ToArray();

	public static (Type @class, A attr)[] GetTypesWithAttribute<A>() where A : Attribute
	{
		List<(Type @class, A attr)> ret = new List<(Type @class, A attr)>();

		IEnumerable<A> attrs;
		foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
		{
			attrs = t.GetCustomAttributes<A>();

			foreach (var attr in attrs)
			{
				ret.Add((t, attr));
			}
		}

		return ret.ToArray();
	}

	public static (C inst, A attr)[] GetInstancesWithAttribute<C, A>() where C : class where A : Attribute
	{
		return GetTypesWithAttribute<A>().Select(d => ( (C) (Activator.CreateInstance(d.@class)), d.attr ) ).ToArray();
	}

	/// <summary>
	/// Uses T's <see cref="ScenePath"/> attribute to load given scene, returning the PackedScene.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static PackedScene LoadScene<T>() where T : class
	{
		var type = typeof(T);
		var attr = type.GetCustomAttribute<ScenePath>();

		if (attr == null)
		{
			throw new ArgumentException($"Parameter {type} to {nameof(LoadScene)} must have attribute {nameof(ScenePath)}.");
		}

		return ResourceLoader.Load<PackedScene>(attr.Path);
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

[AttributeUsage(AttributeTargets.Field)]
public class EnumFloat : Attribute
{
	public readonly float Float;

	public EnumFloat(float @float)
	{
		Float = @float;
	}
}	