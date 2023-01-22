using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Pretty much everything in this namespace is gonna be public and easily settable.
/// This is by design to make writing this thing faster and easier, seeing as it's hidden by all the 
/// namespaces to make sure nothing can actually fuck with it.
/// </summary>
namespace PirateInBetween.Game.Dialogue.Tree
{
	public class DialogueTree : Dialogue
	{
		public const string PLAYER_NAME = ":";

		public DialogueCharacter PlayerSpeaker => GetCharacter(PLAYER_NAME);
		private Dictionary<string, DialogueCharacter> Characters = new Dictionary<string, DialogueCharacter>();

		public int? LastChoice = null;

		public IResponds Current;


		public DialogueTree()
		{
			Current = new Start(this);
		}

		public void AddCharacter(string name, DialogueCharacter character)
		{
			Characters[name] = character;
		}

		public DialogueCharacter GetCharacter(string name)
		{
			return Characters[name];
		}

		protected override DialogueResponse GetNextResponse(int? choice = null)
		{
			LastChoice = choice;
			Current = Current.GetNext();
			return Current.Response();
		}

		#region Tree generation

		public static DialogueTree StringToTree(string[] text)
		{
			DialogueTree ret = new DialogueTree();
			// chain works like a stack trace, at its peek is our topmost choice level
			Stack<ChoiceSelection> chain = new Stack<ChoiceSelection>();
			
			BasicNode lastNode = (BasicNode)ret.Current;
			Node currentNode;

			foreach (string line in text)
			{
				if (ret.TryGenerateNodeFromText(line, out currentNode))
				{
					if (currentNode is ChoiceSelection selection)
					{
						lastNode.NextNode = currentNode;
						chain.Push(selection);
					}
					else if (currentNode is ChoiceStart start)
					{
						if (chain.Peek().ChoiceCount() > 0)
						{
							lastNode.NextNode = chain.Peek().GetEnd();
						}
						lastNode = start;
						chain.Peek().AddChoice(start);
					}
					else if (currentNode is ChoiceEnd end)
					{
						end = chain.Pop().GetEnd();
						lastNode.NextNode = end;
						lastNode = end; 
					}

					else if (currentNode is BasicNode basic)
					{
						lastNode.NextNode = basic;
						lastNode = basic;
					}
					else
					{
						throw new NotImplementedException();
					}

				}
			}

			lastNode.NextNode = new End(ret);

			return ret;
		}

		private bool TryGenerateNodeFromText(string from, out Node node)
		{
			node = null;

			from = from.Trim();

			if (from.Length == 0)
			{
				return false;
			}

			string[] split = from.Split(' ');

			
			// beginning of a choice
			if (split[0] == "::")
			{
				node = new ChoiceStart(this, from.Substring(2));
			}
			// a line of player dialogue
			else if (split[0] == ":")
			{
				node = new CharacterLine(this, from.Substring(1).TrimStart(), PLAYER_NAME);
			}
			// a line of generic dialogue
			else if (split[0].Last() == ':')
			{
				node = new CharacterLine(this, from.Substring(split[0].Length).TrimStart(), split[0]);
			}
			// adding character
			else if (split[0] == "character")
			{
				DialogueCharacter @char = DialogueCharacter.Load();//DialogueCharacter.Load(split[1]);
				node = new AddCharacter(this, split.Length == 4 ? $"{split[3]}:" : PLAYER_NAME, @char);
			}
			// beginning of choice selection
			else if (split[0] == "{")
			{
				node = new ChoiceSelection(this);
			}
			// signifies the end of a choice. ChoiceEnd is discarded, but we still need something to tell us we've reached it
			else if (split[0] == "}")
			{

				node = new ChoiceEnd(this);
			}

			return node != null;
		}

		#endregion
	}

	public interface IResponds
	{
		/// <summary>
		/// The <see cref="DialogueResponse"/> this <see cref="Node"/> returns. This method is called only if
		/// <see cref="Node.ProcessReturnThisIfIsIRespondsElseNext"/> returns itself.
		/// </summary>
		/// <returns></returns>
		DialogueResponse Response();

		/// <summary>
		/// Returns the Node that should be processed after this response finishes.
		/// </summary>
		/// <returns></returns>
		IResponds GetNext();
	}

	public abstract class Node
	{
		public Node(DialogueTree tree)
		{
			DialogueTree = tree;
		}

		/// <summary>
		/// The <see cref="PirateInBetween.Game.Dialogue.Tree.DialogueTree"/> that created this <see cref="Node"/>.
		/// </summary>
		/// <value></value>
		protected DialogueTree DialogueTree { get; private set; }


		// the method name is stupid so i don't forget how it works.
		/// <summary>
		/// Performs any actions associated with this type of Node.
		/// It should return itself if it is of type IResponds, otherwise it should return the next node.
		/// </summary>
		/// <returns></returns>
		public abstract IResponds ProcessReturnThisIfIsIRespondsElseNext();
	}

	public abstract class BasicNode : Node
	{
		public BasicNode(DialogueTree tree) : base(tree)
		{ }

		public Node NextNode;
	}
}