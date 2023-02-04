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

		private Combo _combo;
		private ComboInputContainer _input;
		private bool _requiresInput => _input == null;

		public void GenerateTree(IEnumerable<ComboInputContainer> inputs, Combo finalCombo) => GenerateTree(inputs.Reverse().GetEnumerator(), finalCombo);
		private void GenerateTree(IEnumerator<ComboInputContainer> inputs, Combo finalCombo)
		{
			if (inputs.MoveNext())
			{
				ComboTreeNode next;

				if (!TryFindNodeWith(this, inputs.Current, out next))
				{
					next = AddNodeWith(inputs.Current);
				}

				next.GenerateTree(inputs, finalCombo);
			}
			else
			{
				if (_combo == null)
				{
					_combo = finalCombo;
				}
				else
				{
					throw new InvalidOperationException($"Cannot add combo {nameof(finalCombo)} with same inputs as {nameof(_combo)}.");
				}
			}
		}

		public Combo ParseInput(LinkedList<ComboInputContainer> inputs) => ParseInput(this, inputs);
		public static Combo ParseInput(ComboTreeNode source, IEnumerable<ComboInputContainer> inputs)
		{
			GD.Print($"scanning {inputs.Count()} elements");

			ComboTreeNode currentNode = source;
			ComboTreeNode potentialNext;

			// we're very likely to break out of this early
			foreach (ComboInputContainer input in inputs)
			{
				GD.Print($"current {input}");
				// if we've hit a combo we take it
				if (currentNode._combo != null)
				{

					GD.Print($"found combo {currentNode._combo}");
					return currentNode._combo;
				}

				// try to find a precisely matching input
				if (TryFindNodeWith(currentNode, input, out potentialNext))
				{

					GD.Print($"found precisely matching node {potentialNext}");
					currentNode = potentialNext;
					continue;
				}

				// try to see if this could be a last input
				foreach (ComboTreeNode conn in currentNode._connections)
				{
					if (conn._combo != null && input.EqualsInput(conn._input))
					{
						GD.Print($"found imprecise match {conn}");
						return conn._combo;
					}
				}

				GD.Print("found nothing");
				// we can't find a path out of this node
				break;
			}

			return currentNode?._combo;
		}

		private Combo ParseInput(IEnumerator<ComboInputContainer> inputs)
		{
			if (_combo != null)
			{
				return _combo;
			}

			if (inputs.MoveNext() && TryFindNodeWith(this, inputs.Current, out var node))
			{
				return node.ParseInput(inputs);
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

		
		private ComboTreeNode AddNodeWith(ComboInputContainer input)
		{
			_connections.Add(new ComboTreeNode() { _input = input });
			return _connections.Last();
		}

		public override string ToString()
		{
			return $"Input: ( {_input} ), Combo: ( {_combo} ); ( {string.Join(", ", _connections)} )";
		}
	}
}