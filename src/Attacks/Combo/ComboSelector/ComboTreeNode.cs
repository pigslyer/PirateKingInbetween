using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos.Tree
{
	public class ComboTreeNode
	{
		private List<ComboTreeNode> _connections = new List<ComboTreeNode>();
		private SortedDictionary<ComboAttr, Combo> _combos = new SortedDictionary<ComboAttr, Combo>();

		private ComboInputContainer _input;

		public void GenerateTree(IEnumerable<ComboInputContainer> inputs, (ComboAttr attr, Combo finalCombo) combo) => GenerateTree(inputs.Reverse().GetEnumerator(), combo);
		private void GenerateTree(IEnumerator<ComboInputContainer> inputs, (ComboAttr attr, Combo finalCombo) combo)
		{
			if (inputs.MoveNext())
			{
				ComboTreeNode next;

				if (!TryFindNodeWith(this, inputs.Current, out next))
				{
					next = AddNodeWith(inputs.Current);
				}

				next.GenerateTree(inputs, combo);
			}
			else
			{
				if (_combos.TryGetValue(combo.attr, out var value))
				{
					throw new InvalidOperationException($"Cannot add combo {combo.finalCombo.GetType()} with same signature as {value.GetType()}.");
				}
				else
				{
					_combos.Add(combo.attr, combo.finalCombo);
				}
			}
		}

		public Combo ParseInput(LinkedList<ComboInputContainer> inputs, IComboSelectorStandard state) => ParseInput(this, inputs, state);
		public static Combo ParseInput(ComboTreeNode source, IEnumerable<ComboInputContainer> inputs, IComboSelectorStandard standard)
		{
			ComboTreeNode.PrintDebug($"scanning {inputs.Count()} elements");

			ComboTreeNode currentNode = source;
			ComboTreeNode potentialNext;

			Combo validCombo;

			// we're very likely to break out of this early
			foreach (ComboInputContainer input in inputs)
			{
				ComboTreeNode.PrintDebug($"current {input}");
				// if we've hit a combo we take it
				//if (state.CanUseCombo(currentNode._comboAttr, currentNode._combo))
				if (TryFindValidCombo(currentNode, standard, out validCombo))
				{
					ComboTreeNode.PrintDebug($"found combo {validCombo}");
					return validCombo;
				}

				// try to find a precisely matching input
				if (TryFindNodeWith(currentNode, input, out potentialNext))
				{

					ComboTreeNode.PrintDebug($"found precisely matching node {potentialNext}");
					currentNode = potentialNext;
					continue;
				}

				// try to see if this could be a last input
				foreach (ComboTreeNode conn in currentNode._connections)
				{
					if (input.EqualsInput(conn._input) && TryFindValidCombo(conn, standard, out validCombo))
					{
						ComboTreeNode.PrintDebug($"found imprecise match {validCombo}");
						return validCombo;
					}
				}

				ComboTreeNode.PrintDebug("found nothing");
				// we can't find a path out of this node
				break;
			}
			
			// if we have N inputs and a perfectly inputted combo of N inputs, then we need to check N+1 times
			if (TryFindValidCombo(currentNode, standard, out validCombo))
			{
				return validCombo;
			}

			return null;
		}

		private static bool TryFindNodeWith(ComboTreeNode on, ComboInputContainer input, out ComboTreeNode node)
		{
			for (int i = 0; i < on._connections.Count; i++)
			{
				if (input == on._connections[i]._input)
				{
					node = on._connections[i];
					return true;
				}
			}

			node = null;
			return false;
		}

		private static bool TryFindValidCombo(ComboTreeNode on, IComboSelectorStandard standard, out Combo validCombo)
		{
			foreach (var pair in on._combos)
			{
				if (standard.CanUseCombo(pair.Key, pair.Value))
				{
					validCombo = pair.Value;
					return true;
				}
			}

			validCombo = null;
			return false;
		}
		
		private ComboTreeNode AddNodeWith(ComboInputContainer input)
		{
			_connections.Add(new ComboTreeNode() { _input = input });
			return _connections.Last();
		}

		public override string ToString()
		{
			return $"Input: ( {_input} ), Combo count: ( { _combos.Count } ); ( {string.Join(", ", _connections)} )";
		}

		public static void PrintDebug(string message)
		{
#if COMBO_TREE_DEBUG
			GD.Print(message);
#endif
		}
	}
}