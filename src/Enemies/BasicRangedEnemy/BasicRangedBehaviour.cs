using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Enemies.Behaviours
{
	public class BasicRangedBehaviour : GroundlingBehaviour
	{
		const float REACTION_TIME = 2f;

		protected override void Execute()
		{
			if (TrySeeOpponent(DamageDealerTargettingArea.RangedSight, REACTION_TIME, out var _))
			{
				
				return;
			}
			if (TrySeeOpponent(DamageDealerTargettingArea.RangedSight, out var target))
			{
				if (GenericRay.CastRay(toGlobal: target.GlobalPosition, mask: PhysicsLayers.World))
				{
					ResetTimeSeen();
				}
				else
				{
					StopWandering(REACTION_TIME * 2);

					Wander(PatrolMaxSpeed, PatrolMovementAcceleration * 2, (2f, 6f), (6f, 8f), false);
					return;
				}
			}

			Wander(PatrolMaxSpeed, PatrolMovementAcceleration, (2f, 6f), (6f, 8f), true);
		}
	}
}