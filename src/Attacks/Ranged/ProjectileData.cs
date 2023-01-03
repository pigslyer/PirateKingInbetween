using Godot;
using System;
using MonoCustomResourceRegistry;

namespace PirateInBetween.Game
{
	[RegisteredType(nameof(ProjectileData))]
	public class ProjectileData : AttackData
	{
		[Export] public PackedScene BulletScene { get; private set; } = null;

		[Export] public int Damage { get; private set; } = 1;
	}
}