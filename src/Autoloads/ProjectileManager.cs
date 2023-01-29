using Godot;
using System;

namespace PirateInBetween.Game.Autoloads
{
	public class ProjectileManager : Autoload<ProjectileManager>
	{
		
		public static T SetupProjectile<T>(T projectile, Vector2 position, MovingParent newParent) where T : Node2D, IProjectile
		{
			newParent.AddChild(projectile);	
			projectile.GlobalPosition = position;

			return projectile;
		}
		

		public static void ClearProjectiles()
		{
			foreach (var child in Instance.GetChildren())
			{
				(child as Node).QueueFree();
			}
		}
	}
}