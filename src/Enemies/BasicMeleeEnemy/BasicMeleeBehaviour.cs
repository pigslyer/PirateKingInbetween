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
		protected override void Execute()
		{
			if (TrySeeOpponent(DamageDealerTargettingArea.Front, out var _))
			{
				ExecuteCombo<BasicCombo1>();

				return;
			}

			Wander(PatrolMaxSpeed, PatrolMovementAcceleration, (2f, 6f), (6f, 8f), true);
		}
	}
}