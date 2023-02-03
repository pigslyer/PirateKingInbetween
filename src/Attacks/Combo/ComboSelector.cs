using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	public class ComboSelector : Node
	{
		private class ComboTreeNode
		{
			private List<ComboTreeNode> _connections = new List<ComboTreeNode>(); 

			public Combo Combo;
			public ComboInputContainer Input;
			public bool RequiresInput => Input == null;

			public void GenerateTree(IEnumerable<ComboInputContainer> inputs, Combo finalCombo) => GenerateTree(inputs.Reverse().GetEnumerator(), finalCombo);
			private void GenerateTree(IEnumerator<ComboInputContainer> inputs, Combo finalCombo)
			{
				if (inputs.MoveNext())
				{
					ComboTreeNode next;

					if (!TryFindNodeWith(inputs.Current, out next))
					{
						next = AddNodeWith(inputs.Current);
					}

					next.GenerateTree(inputs, finalCombo);
				}
				else
				{
					if (Combo == null)
					{
						Combo = finalCombo;
					}
					else
					{
						throw new InvalidOperationException($"Cannot add combo {nameof(finalCombo)} with same inputs as {nameof(Combo)}.");
					}
				}
			}

			public Combo ParseInput(IEnumerable<ComboInputContainer> inputs) => ParseInput(inputs.GetEnumerator());
			private Combo ParseInput(IEnumerator<ComboInputContainer> inputs)
			{
				if (Combo != null)
				{
					return Combo;
				}

				if (inputs.MoveNext() && TryFindNodeWith(inputs.Current, out var node))
				{
					return node.ParseInput(inputs);
				}

				return null;
				//Inputs.MoveNext() ? FindNodeWith(inputs.Current)?.ParseInput(inputs) : Combo;
			}

			private bool TryFindNodeWith(ComboInputContainer input, out ComboTreeNode node)
			{
				foreach (ComboTreeNode n in _connections)
				{
					if (n.Input == input)
					{
						node = n;
						return true;
					}
				}

				node = null;
				return false;
			}
			private ComboTreeNode AddNodeWith(ComboInputContainer input)
			{
				_connections.Add(new ComboTreeNode(){Input = input});
				return _connections.Last();
			}

			public override string ToString()
			{
				return $"Input:( {Input} ), Combo:( {Combo} ); ( {string.Join(", ", _connections)} )";
			}
		}

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
			GD.Print(string.Join(", ", _registered));
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

		public void RegisterEvent(ComboInput input, ICombatFrameData data)
		{
			float delta = _currentTime - _lastRegisteredTime;
			GD.Print($"Delta: {delta}");

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
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			_currentTime += delta;
		}
	}
}