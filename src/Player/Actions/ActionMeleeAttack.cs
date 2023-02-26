using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Actions
{
	public class ActionMeleeAttack : PlayerAction
	{
		public readonly DamageData DamageData; 
		public readonly float SlashDuration;

		public ActionMeleeAttack(DamageData damageData, float slashDuration) : base(Animations.WIP)
		{
			DamageData = damageData; SlashDuration = slashDuration;
		}
	}
}