using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Behaviours
{
	public class PlayerDamageHandler : PlayerBehaviour
	{
		public DamageReaction TakeDamage()
		{
			if (IsEnabled())
			{
				return DamageReaction.One;
			}

			return DamageReaction.Zero;
		}

		public void StunFor(float time)
		{
			
		}

		public override void Run(PlayerCurrentFrameData data)
		{ 

		}
	}
}