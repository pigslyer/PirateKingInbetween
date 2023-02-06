using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// The same physics layers as defined in Project Settings.
/// </summary>
[Flags]
public enum PhysicsLayers : uint
{
	None = 0,
	World = 1,
	Player = 2,
	WorldHittable = 4,
	CarriableBox = 8,
	Interaction = 16,
}
