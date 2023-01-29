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
		CarriableBox = 8,
		Interaction = 16,
	}

	/// <summary>
	/// Convenience class that casts CollisionLayer and CollisionMask to PhysicsLayers and vice versa.
	/// </summary>
	public abstract class Area2D : Godot.Area2D
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
	public abstract class KinematicBody2D : Godot.KinematicBody2D
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
	public abstract class StaticBody2D : Godot.StaticBody2D
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