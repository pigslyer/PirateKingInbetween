using Godot;
using System;

namespace PirateInBetween.Game.Autoloads
{
	public class ProjectileManager : Autoload<ProjectileManager>
	{
		/// <summary>
		///	Adds given bullet to the scene tree at position. This method should always be called by the bullet itself, not the thing instancing it.
		/// </summary>
		public T Setup<T>(T bullet, Vector2 position) where T : BulletBase
		{
			AddChild(bullet);
			bullet.Position = position;

			return bullet;
		}

		public void ClearBullets()
		{
			foreach (var child in GetChildren())
			{
				(child as Node).QueueFree();
			}
		}
	}
}