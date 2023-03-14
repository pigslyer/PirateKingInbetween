using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos.List;

namespace PirateInBetween.Game.Enemies.Behaviours
{
	public class BasicMeleeBehaviour : GroundlingBehaviour
	{
		private const float REACTION_TIME = 1.5f;

		protected override void Execute()
		{
			if (TrySeeOpponent(DamageDealerTargettingArea.Front, REACTION_TIME, out var _))
			{
				ExecuteCombo<BasicCombo1>();

				return;
			} 
			
			if (TrySeeOpponent(DamageDealerTargettingArea.Front, out var _))
			{
				CurrentData.IsAboutToAttack = true;
				StopWandering(REACTION_TIME * 2);

				Wander(PatrolMaxSpeed, PatrolMovementAcceleration * 2, (2f, 6f), (6f, 8f), false);
				return;
			}

			Wander(PatrolMaxSpeed, PatrolMovementAcceleration, (2f, 6f), (6f, 8f), true);
		}
	}
}