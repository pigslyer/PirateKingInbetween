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
	public class ComboSelector : Node
	{
		private static readonly ComboTreeNode _root;

		static ComboSelector()
		{
			_root = new ComboTreeNode();

			foreach (var pair in Combo.Combos)
			{
				_root.GenerateTree(pair.attr.Inputs, pair.combo);
			}

		
			GD.Print(_root);
		}

		
		public Combo GetSelected()
		{
			Combo combo = _root.ParseInput(_registered);
			
			if (combo != null)
			{
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
				int diff = (InputManager.IsActionJustPressed(InputButton.MoveRight) ? 1 : 0) - (InputManager.IsActionJustPressed(InputButton.MoveLeft) ? 1 : 0);

				if (diff != 0)
				{
					det = (diff == 1) == data.FacingRight ? ComboInput.Forward : ComboInput.SwitchDirection;
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
			float delta = _currentTime - _lastRegisteredTime;
			//GD.Print($"Delta: {delta}");

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

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			_currentTime += delta;
		}
	}
}