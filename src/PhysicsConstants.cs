using Godot;
using System;

namespace PirateInBetween
{
	/// <summary>
	/// The same physics layers as defined in Project Settings.
	/// </summary>
	[Flags] public enum PhysicsLayers : uint
	{
		None = 0,
		World = 1,
		Player = 2,
		WorldHittable = 4,
	}

	/// <summary>
	/// Convenience class that casts CollisionLayer and CollisionMask to PhysicsLayers and vice versa.
	/// </summary>
	public abstract class Area2DOverride : Area2D
	{
		public new PhysicsLayers CollisionLayer { 
			get => (PhysicsLayers) base.CollisionLayer; 
			set => base.CollisionLayer = (uint) value; 
		}
		
		public new PhysicsLayers CollisionMask{
			get => (PhysicsLayers) base.CollisionMask;
			set => base.CollisionMask = (uint) value;
		}
	}

	public abstract class KinematicBody2DOVerride : KinematicBody2D
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
}