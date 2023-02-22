using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Enemies.Behaviours
{
	public class BasicMeleeBehaviour : GroundlingBehaviour
	{
		protected override void Execute()
		{
			Wander(PatrolMaxSpeed, PatrolMovementAcceleration, (2f, 6f), (6f, 8f), true);
		}
	}
}