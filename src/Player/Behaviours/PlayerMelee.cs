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
		[Export] private NodePath __lastDisplayPath = null;

		private ComboSelector _selector = new ComboSelector();
		private Combo _currentCombo = null;
		private Combo _attemptingCombo = null;
		private PlayerCurrentFrameData _lastData = null;

//		private const Behaviours MidSlashDisabled = Behaviours.RangedAttack | Behaviours.Jumping | Behaviours.Interaction | Behaviours.Carrying;
		private const Behaviours MidComboDisabled = Behaviours.Default & ~Behaviours.MeleeAttack;

		public override void _Ready()
		{
			base._Ready();

			AddChild(_selector);
		}

		public override void Run(PlayerCurrentFrameData data)
		{
			_lastData = data;
			if (CanChangeActive && !IsInControl && _attemptingCombo != null)
			{
				GD.Print($"began executing {_attemptingCombo}");
				_currentCombo = _attemptingCombo;

				ExecuteBehaviour(data);
				data.CurrentAction = PlayerAnimation.MeleeAttack;
			} 
			else if (IsInControl)
			{
				_currentCombo.GiveFrameData(data);
				data.CurrentAction = PlayerAnimation.MeleeAttack;
			}
		}

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);

			if (IsEnabled(Behaviours.MeleeAttack) && CanChangeActive && !IsInControl && _attemptingCombo == null && _currentCombo == null && _lastData != null && ComboSelector.TryDetectInput(_lastData, out ComboInput detected))
			{
				//GD.Print($"detected input: {detected}");
				TrackEvent(_selector.RegisterEvent(detected, _lastData));
				_attemptingCombo = _selector.GetSelected();
			}
		}

		private async Task ExecuteBehaviour(PlayerCurrentFrameData data)
		{
			SetBehaviourChangesDisabled(true);
			SetBehavioursEnabled(MidComboDisabled, false);

			await _currentCombo.ExecuteCombo(GetPlayer(), data);

			SetBehaviourChangesDisabled(false);
			SetBehavioursEnabled(MidComboDisabled, true);
			
			_currentCombo = _attemptingCombo = null;
		}

		private const int TRACKED_COUNT = 2;
		private LinkedList<ComboInputContainer> _inputs = new LinkedList<ComboInputContainer>();

		private void TrackEvent(ComboInputContainer input)
		{
			_inputs.AddLast(input);

			if (_inputs.Count > TRACKED_COUNT)
			{
				_inputs.RemoveFirst();
			}

			GetNode<Label>(__lastDisplayPath).Text = string.Join("\n", _inputs);
		}
	}
}