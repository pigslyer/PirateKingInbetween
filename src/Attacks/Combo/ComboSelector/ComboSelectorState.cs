using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos.Tree;

namespace PirateInBetween.Game.Combos
{
	public partial class ComboSelector
	{

		private class ComboSelectorState : IComboSelectorStandard
		{
			private readonly ComboSelector _selector;
			private Type _lastCombo;
			private ICombatFrameData _lastData;
			private float _currentTime;
			private IComboExecutor _executor;

			private bool MovingForward => _lastData.IsMoving();

			public ComboSelectorState(ComboSelector selector)
			{
				_selector = selector;
			}
			public void SetExecutor(IComboExecutor executor) => _executor = executor;
			public void SetLastData(ICombatFrameData data) => _lastData = data;

			public bool CanUseCombo(ComboAttr attr, Combo combo)
			{
				if (attr.LastRequiredCombo != null && attr.LastRequiredCombo != _lastCombo)
				{
					ComboTreeNode.PrintDebug("last required combo");
					return false;
				}

				if (!attr.MustHoldForward.Evaluate(MovingForward))
				{
					ComboTreeNode.PrintDebug("must hold forward");
					return false;
				}

				if (!attr.MustBeOnFloor.Evaluate(_executor.IsOnFloor))
				{
					ComboTreeNode.PrintDebug("must be on floor");
					return false;
				}

				if (!attr.IntervalSinceLastCombo.IsInRange(_currentTime))
				{
					ComboTreeNode.PrintDebug($"{_currentTime} not in interval {attr.IntervalSinceLastCombo}");
					return false;
				}

				return true;
			}
			
			

			public void UsingCombo(Combo combo)
			{
				_lastCombo = combo.GetType();

				combo.OnFinished(() => _currentTime = 0f);
			}

			public void Process(float delta)
			{
				_currentTime += delta;
			}
		}
	}
	
}