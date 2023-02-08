using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[AttributeUsage(AttributeTargets.Class)]
public class ScenePath : Attribute
{
	public readonly string Path;

	public ScenePath(string path)
	{
		Path = path;
	}
}