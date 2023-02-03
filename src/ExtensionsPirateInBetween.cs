using System.Threading.Tasks;
using Godot;
using PirateInBetween;

public static class ExtensionsPirateInBetween
{
	
	#region Overloads

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
			ray.CastTo = ray.ToLocal((Vector2) toGlobal);
		}
		else if (toLocal != null)
		{
			ray.CastTo = (Vector2) toLocal;
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

		collidingWith = ret ? (T) ray.GetCollider() : null;

		return ret;
	}

	#endregion
}