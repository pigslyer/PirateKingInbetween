using Godot;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

/// <summary>
/// Universally useful extensions, no need for them to occupy any namespace. Partial to allow game specific additions.
/// </summary>
public static class ExtensionsGodot
{
	/// <summary>
	/// Just overloads SetDeferred so i don't have to type the same shit all the time.
	/// </summary>
	public static async Task SetLayer(this CollisionObject2D node, PhysicsLayers layer)
	{
		node.SetDeferred("collision_layer", (int)layer);
		await node.ToSignal(node.GetTree(), "physics_frame");
	}

	/// <summary>
	/// Just overloads SetDeferred so i don't have to type the same shit all the time.
	/// </summary>
	public static async Task SetMask(this CollisionObject2D node, PhysicsLayers mask)
	{
		node.SetDeferred("collision_mask", (int)mask);
		await node.ToSignal(node.GetTree(), "physics_frame");
	}

	/// <summary>
	/// Overloads the old yield(get_tree(), "idle_frame");
	/// </summary>
	public static async Task AwaitIdle(this Node node)
	{
		await node.ToSignal(node.GetTree(), "idle_frame");
	}

	/// <summary>
	/// Overloads the old yield(get_tree(), "physics_frame");
	/// </summary>
	public static async Task AwaitPhysics(this Node node)
	{
		await node.ToSignal(node.GetTree(), "physics_frame");
	}

	public static float GetPercentDone(this Timer timer)
	{
		return 1f - timer.TimeLeft/timer.WaitTime;
	}


	/// <summary>
	/// Waits for alloted time.
	/// </summary>
	public static async Task WaitFor(this Node node, float time) => await WaitFor(node, time, () => true);

	/// <summary>
	/// Much like its functionless pair, but this one also checks the given condition. If given condition is ever false, timer preemptively stops
	/// </summary>
	public static async Task WaitFor(this Node node, float time, Func<bool> orFor)
	{
		if (time > 0f)
		{
			float t = 0f;
			while (t < time && !node.IsQueuedForDeletion() && orFor())
			{
				await node.ToSignal(node.GetTree(), "idle_frame");

				t += node.GetProcessDeltaTime();
			}
		}
	}

	public static void Reparent(this Node child, Node newParent)
	{
		if (child.GetParent() != null)
		{
			child.GetParent().RemoveChild(child);
		}
		newParent.AddChild(child);
	}

	public static void Reparent2D(this Node2D child, Node newParent)
	{
		if (child.GetParent() != null)
		{
			Vector2 temp = child.GlobalPosition;
			Reparent(child, newParent);
			child.GlobalPosition = temp;
		}
		else
		{
			Reparent(child, newParent);
		}
	}

	public static Vector2 ToVectorDeg(this float deg, float speed) => ToVectorDeg(deg) * speed;
	public static Vector2 ToVectorDeg(this float deg) => ToVectorRad(Mathf.Deg2Rad(deg));
	public static Vector2 ToVectorRad(this float rad) => new Vector2(Mathf.Cos(rad), -Mathf.Sin(rad));

	public static T[] ToArray<T>(this Godot.Collections.Array array) => array.Cast<T>().ToArray();

	public static Godot.Area2D[] GetAreas(this Godot.Area2D area) => area.GetOverlappingAreas().ToArray<Godot.Area2D>();

	public static T Min<T>(this IEnumerable<T> enumerable, Func<T, int> standard) where T : Node2D
	{
		IEnumerator<T> e = enumerable.GetEnumerator();
		e.Reset();
		
		if (!e.MoveNext()) return null;
		T ret = e.Current;
		int val = standard(ret);
		int temp;

		while (e.MoveNext())
		{
			temp = standard(e.Current);

			if (temp < val)
			{
				val = temp;
				ret = e.Current;
			}
		}

		return ret;
	}

	/// <summary>
	/// Casts ray to global position <see cref="toGlobal"/>, using mask <see cref="mask"/>. If either is null, the current is kept.
	/// The function returns true if the ray now collides with anything.
	/// </summary>
	/// <param name="ray"></param>
	/// <param name="toGlobal"></param>
	/// <param name="mask"></param>
	/// <returns></returns>
	public static bool CastRay(this RayCast2D ray, Vector2? toGlobal = null, Vector2? toLocal = null, PhysicsLayers? mask = null)
	{
		if (toGlobal != null)
		{
			ray.CastTo = ray.ToLocal((Vector2)toGlobal);
		}
		else if (toLocal != null)
		{
			ray.CastTo = (Vector2)toLocal;
		}

		if (mask != null)
		{
			ray.CollisionMask = (uint)mask;
		}

		ray.ForceRaycastUpdate();

		return ray.IsColliding();
	}

	/// <summary>
	/// Casts ray to global position <see cref="to"/>, using mask <see cref="mask"/>. If either is null, the current is kept.
	/// The function returns true if the ray now collides with anything.
	/// <see cref="collidingWith"/> represents the first object the newly updated ray intersects.
	/// </summary>
	public static bool TryCollideRay<T>(this RayCast2D ray, out T collidingWith, Vector2? toGlobal = null, Vector2? toLocal = null, PhysicsLayers? mask = null) where T : CollisionObject2D
	{
		bool ret = CastRay(ray, toGlobal, toLocal, mask);

		collidingWith = ret ? (T)ray.GetCollider() : null;

		return ret;
	}

}