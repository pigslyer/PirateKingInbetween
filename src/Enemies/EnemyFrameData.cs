using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;

namespace PirateInBetween.Game.Enemies
{
	public class EnemyFrameData : ICombatFrameData
	{
		public Vector2 Velocity;

		public float Delta { get; set; }

		public bool FacingRight;

		public bool IsMoving() => Velocity.LengthSquared() > 0;

		#region ICombatFrameData implementation
		
		Vector2 ICombatFrameData.Velocity { get => Velocity; set => Velocity = value; }
		bool ICombatFrameData.FacingRight { get => FacingRight; }
		void ICombatFrameData.SwitchDirection() => FacingRight = !FacingRight;
		

		#endregion
	}
}