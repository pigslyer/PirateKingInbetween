using Godot;
using System;

namespace PirateInBetween.Player
{
	public abstract class PlayerState : Node
	{

		// This is only for getting state
		protected PlayerController Player {get; private set;}

		public void Initialize(PlayerController player)
		{
			Player = player;
		}
	}
}