using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Convenience class that casts CollisionLayer and CollisionMask to PhysicsLayers and vice versa.
/// </summary>
public class Area2D : Godot.Area2D
{
	public new PhysicsLayers CollisionLayer 
	{ 
		get => (PhysicsLayers) base.CollisionLayer; 
		set => base.CollisionLayer = (uint) value; 
	}
	
	public new PhysicsLayers CollisionMask
	{
		get => (PhysicsLayers) base.CollisionMask;
		set => base.CollisionMask = (uint) value;
	}

	/// <summary>
	/// Controls both monitoring and moniterable. If state is true, both are set to true, otherwise both are false.
	/// </summary>
	/// <param name="state"></param>
	protected void SetEnabled(bool state)
	{
		SetDeferred("monitoring", state);
		SetDeferred("monitorable", state);
	}
}

/// <summary>
/// Convenience class that casts CollisionLayer and CollisionMask to PhysicsLayers and vice versa.
/// </summary>
public class KinematicBody2D : Godot.KinematicBody2D
{
	public new PhysicsLayers CollisionLayer
	{
		get => (PhysicsLayers)base.CollisionLayer;
		set => base.CollisionLayer = (uint)value;
	}

	public new PhysicsLayers CollisionMask
	{
		get => (PhysicsLayers)base.CollisionMask;
		set => base.CollisionMask = (uint)value;
	}
}

/// <summary>
/// Convenience class that casts CollisionLayer and CollisionMask to PhysicsLayers and vice versa.
/// </summary>
public class StaticBody2D : Godot.StaticBody2D
{
	public new PhysicsLayers CollisionLayer
	{
		get => (PhysicsLayers)base.CollisionLayer;
		set => base.CollisionLayer = (uint)value;
	}

	public new PhysicsLayers CollisionMask
	{
		get => (PhysicsLayers)base.CollisionMask;
		set => base.CollisionMask = (uint)value;
	}
}

/// <summary>
/// Convinience class that etc.
/// </summary>
public class RayCast2D : Godot.RayCast2D
{
	public new PhysicsLayers CollisionMask
	{
		get => (PhysicsLayers)base.CollisionMask;
		set => base.CollisionMask = (uint) value;
	}

}