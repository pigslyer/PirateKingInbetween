using Godot;

namespace PirateInBetween.Game
{
	public interface IProjectile
	{
		void AimAt(Vector2 startFrom, Node2D target, ProjectileData data, PhysicsLayers targetting);
	}
}