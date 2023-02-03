using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;
using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player.Behaviours
{
	public class PlayerMelee : PlayerBehaviour
	{	

		private ComboSelector _selector = new ComboSelector();
		private Combo _currentCombo = null;

//		private const Behaviours MidSlashDisabled = Behaviours.RangedAttack | Behaviours.Jumping | Behaviours.Interaction | Behaviours.Carrying;
		private const Behaviours MidComboDisabled = Behaviours.Default & ~Behaviours.MeleeAttack;

		public override void _Ready()
		{
			base._Ready();

			AddChild(_selector);
		}

		public override void Run(PlayerCurrentFrameData data)
		{
			if (CanChangeActive && !IsInControl && ComboSelector.TryDetectInput(data, out ComboInput detected))
			{
				GD.Print($"detected input: {detected}");
				_selector.RegisterEvent(detected, data);
				_currentCombo = _selector.GetSelected();

				if (_currentCombo != null)
				{
					GD.Print($"began executing {_currentCombo}");
					ExecuteBehaviour(data);
					data.CurrentAction = PlayerAnimation.MeleeAttack;
				}
			} 
			else if (IsInControl)
			{
				_currentCombo.GiveFrameData(data);
				data.CurrentAction = PlayerAnimation.MeleeAttack;
			}
		}

		private async Task ExecuteBehaviour(PlayerCurrentFrameData data)
		{
			SetBehaviourChangesDisabled(true);
			SetBehavioursEnabled(MidComboDisabled, false);

			await _currentCombo.ExecuteCombo(GetPlayer(), data);

			SetBehaviourChangesDisabled(false);
			SetBehavioursEnabled(MidComboDisabled, true);
		}

	}
}