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
		private float _stunTimer = 0f;

		public DamageReaction TakeDamage()
		{
			if (IsEnabled())
			{
				return DamageReaction.One;
			}

			return DamageReaction.Zero;
		}

		public void StunFor(float time, PlayerCurrentFrameData data)
		{
			if (time > 0f)
			{
				data.Velocity = Vector2.Zero;
				_stunTimer = time;
				ForceBehaviourChangesDisabled();

				SetBehaviourChangesDisabled(true);
				SetBehavioursEnabled(Behaviours.Controllable, false);
			}
		}

		public void KnockbackFor(Vector2 velocity, PlayerCurrentFrameData data) => data.Velocity = velocity;

		public void DisableStun()
		{
			_stunTimer = 0f;

			SetBehaviourChangesDisabled(false);
			SetBehavioursEnabled(Behaviours.Controllable, true);
		}

		public override void Run(PlayerCurrentFrameData data)
		{
			if (_stunTimer > 0f)
			{
				_stunTimer -= data.Delta;
		
				if (_stunTimer <= 0f)
				{
					DisableStun();
				}
			}
		}
	}
}