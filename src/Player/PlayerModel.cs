using Godot;
using System;

namespace PirateInBetween.Player
{
	public class PlayerModel : Node2D
	{
		
		public void SetAnimation(PlayerAnimation state, bool facingRight)
		{
			Scale = new Vector2(facingRight ? 1f : -1f, 1f);
		}

	}
}

