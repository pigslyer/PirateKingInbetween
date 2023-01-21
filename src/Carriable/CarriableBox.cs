using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class CarriableBox : KinematicBody2DOverride
	{
		public void ApplyVelocity(Vector2 vel)
		{
			MoveAndSlide(vel);
		}
	}
}