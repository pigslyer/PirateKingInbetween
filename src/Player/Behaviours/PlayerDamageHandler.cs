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
		private const Behaviours DISABLED_DURING_STUN = Behaviours.Controllable | Behaviours.AnimationSelector;

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
				if (!IsInControl)
				{
					ForceBehaviourChangesDisabled();
				}

				data.Velocity = Vector2.Zero;
				_stunTimer = time;
				
				SetBehaviourChangesDisabled(true);
				SetBehavioursEnabled(DISABLED_DURING_STUN, false);
			}
		}

		private bool IsStunned() => _stunTimer > 0f;

		public void DisableStun()
		{
			_stunTimer = 0f;

			SetBehaviourChangesDisabled(false);
			SetBehavioursEnabled(DISABLED_DURING_STUN, true);
		}

		public void KnockbackFor(Vector2 velocity, PlayerCurrentFrameData data)
		{
			NotOnFloor();
			data.Velocity = velocity;
		}


		public override void Run(PlayerCurrentFrameData data)
		{
			if (IsStunned())
			{
				if (IsOnFloor())
				{
					data.Velocity.x = 0;
				}

				_stunTimer -= data.Delta;

				if (IsStunned())
				{
					data.CurrentAction = IsOnFloor() ? PlayerAnimation.Stunned : PlayerAnimation.StunnedAir;
				}
				else
				{
					DisableStun();
				}

			}
		}

		public override void ResetState()
		{
			base.ResetState();

			if (IsStunned())
			{
				DisableStun();
			}
		}
	}
}