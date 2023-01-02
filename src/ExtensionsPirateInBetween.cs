using System.Threading.Tasks;
using Godot;
using PirateInBetween;

public static partial class Extensions{
	
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
		
	#endregion
}