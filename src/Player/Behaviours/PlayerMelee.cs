using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player.Behaviours
{
	public class PlayerMelee : PlayerBehaviour
	{
		[Export] private int _slashDamage = 1;
		[Export] private float _slashDuration = 0.6f;
		[Export] private float _slashCooldown = 0.2f;
		[Export] private float _meleeAnimationVelocityMult = 0.1f;
		

		private Task _cooldownTask = null;

		private const Behaviours MidSlashDisabled = Behaviours.RangedAttack | Behaviours.Jumping | Behaviours.Interaction | Behaviours.Carrying;

		public override void Run(PlayerCurrentFrameData data)
		{

			if (!IsInControl && CanChangeActive && !data.IsBusy && _cooldownTask == null && InputManager.IsActionJustPressed(InputButton.MeleeAttack))
			{
				data.CurrentAction = new ActionMeleeAttack(new DamageData(_slashDamage, () => GetPlayer().GlobalPosition), _slashDuration);
				Strike();
			}

			if (IsInControl)
			{
				data.VelocityMult = _meleeAnimationVelocityMult;
				data.CurrentAction = PlayerAnimation.MeleeAttack;
			}
		}


		private async Task Strike()
		{
			SetBehaviourChangesDisabled(true);
			SetBehavioursEnabled(MidSlashDisabled, false);

			await this.WaitFor(_slashDuration);

			SetBehaviourChangesDisabled(false);
			SetBehavioursEnabled(MidSlashDisabled, true);

			_cooldownTask = this.WaitFor(_slashCooldown).ContinueWith(t => _cooldownTask = null);
		}
	}
}