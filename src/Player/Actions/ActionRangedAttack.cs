using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Actions
{
	public class ActionRangedAttack : PlayerAction
	{
		public readonly Projectile Bullet;
		
		public ActionRangedAttack(Projectile bullet) : base(PlayerAnimation.RangedAttack)
		{
			Bullet = bullet;	
		}
	}
}