using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public abstract class PlayerBehaviour : Node
	{

		/// <summary>
		/// This should only ever be used to get state.
		/// </summary>
		protected PlayerController Player {get; private set;}

		public void Initialize(PlayerController player)
		{
			Player = player;
		}

		public abstract void Run(PlayerCurrentFrameData data);

		// these have to be the same order as "States" in the player scene.
		[Flags] public enum Behaviours
		{
			None = 0,
			Movement = 1,
			MeleeAttack = 2,
			RangedAttack = 4,
			Noclip = 8,
			Default = ~0 - Noclip,
		}
	}
}