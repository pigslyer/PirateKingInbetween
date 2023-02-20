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
	WorldHittable = 2,
	Player = 4,
	PlayerHittable = 8,
	Enemy = 16,
	EnemyHittable = 32,
	CarriableBox = 64,
	Interaction = 128,
}
