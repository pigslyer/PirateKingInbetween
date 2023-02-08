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
	public partial class ComboSelector : Node
	{
		private static readonly ComboTreeNode _root;
		private ComboSelectorState _state;

		static ComboSelector()
		{
			_root = new ComboTreeNode();

			foreach (var pair in ReflectionHelper.GetInstancesWithAttribute<Combo, ComboAttr>())
			{
				_root.GenerateTree(pair.attr.Inputs, (pair.attr, pair.inst));
			}

			GD.Print(_root);
		}

		public ComboSelector()
		{
			_state = new ComboSelectorState(this);
		}
		
		public Combo GetSelected(IComboExecutor potentialExecutor)
		{
			_state.SetExecutor(potentialExecutor);
			Combo combo = _root.ParseInput(_registered, _state);
			
			if (combo != null)
			{
				_state.UsingCombo(combo);
				_registered.Clear();
			}
			
			return combo;
		}

		private const int NUMBER_OF_TRACKED_INPUTS = 10;
		private const float TIME_UNTIL_REFRESH = 0.8f;

		private LinkedList<ComboInputContainer> _registered = new LinkedList<ComboInputContainer>();
		private float _lastRegisteredTime = 0f;
		private float _currentTime = 0f;

		public static bool TryDetectInput(ICombatFrameData data, out ComboInput detected)
		{
			ComboInput? det = null;

			// horizontal
			{
				// xor ensures that we don't continue if both have been pressed (however that could have happened)
				if (InputManager.IsActionJustPressed(InputButton.MoveLeft) != InputManager.IsActionJustPressed(InputButton.MoveRight))
				{
					det = data.IsJustGoingForward() ? ComboInput.Forward : ComboInput.SwitchDirection;
				}
			}

			// vertical
			{
				if (InputManager.IsActionJustPressed(InputButton.MoveUp))
				{
					det = ComboInput.Up;
				} 
				else if (InputManager.IsActionJustPressed(InputButton.MoveDown))
				{
					det = ComboInput.Down;
				}
			}

			// attacks
			{
				if (InputManager.IsActionJustPressed(InputButton.MeleeAttack))
				{
					det = ComboInput.Melee;
				}
				else if (InputManager.IsActionJustPressed(InputButton.RangedAttack))
				{
					det = ComboInput.Ranged;
				}
			}

			detected = det ?? ComboInput.Forward;
			return det != null;
		}

		public ComboInputContainer RegisterEvent(ComboInput input, ICombatFrameData data)
		{
			_state.SetLastData(data);
			
			float delta = _currentTime - _lastRegisteredTime;

			if (delta > TIME_UNTIL_REFRESH)
			{
				_registered.Clear();
				delta = 0f;
				_currentTime = 0f;
			}

			_registered.AddFirst(new ComboInputContainer(input, delta));

			if (_registered.Count > NUMBER_OF_TRACKED_INPUTS)
			{
				_registered.RemoveLast();
			}

			_lastRegisteredTime = _currentTime;

			return _registered.First();
		}

		public override void _Process(float delta)
		{
			base._Process(delta);

			_currentTime += delta;
			_state.Process(delta);
		}

		
	}
}