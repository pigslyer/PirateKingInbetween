using Godot;
using System;

namespace PirateInBetween.Player
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
	}
}