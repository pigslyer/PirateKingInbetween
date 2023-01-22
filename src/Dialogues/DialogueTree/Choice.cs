using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue.Tree
{
	public class ChoiceSelection : Node, IResponds
	{
		private readonly ChoiceEnd _end;
		private readonly List<ChoiceStart> _next = new List<ChoiceStart>();

		public ChoiceSelection(DialogueTree tree) : base(tree)
		{ 
			_end = new ChoiceEnd(DialogueTree);
		}

		public void AddChoice(ChoiceStart choice)
		{
			_next.Add(choice);
		}

		public int ChoiceCount() => _next.Count;

		public ChoiceEnd GetEnd() => _end;


		public override IResponds ProcessReturnThisIfIsIRespondsElseNext()
		{	
			return this;
		}
		public DialogueResponse Response()
		{
			return new DialogueResponse(DialogueTree.PlayerSpeaker, _next.Select(choice => choice.Text).ToArray());
		}
		public IResponds GetNext() => _next[(int)DialogueTree.LastChoice].ProcessReturnThisIfIsIRespondsElseNext();

		public override string ToString() => string.Join(", ", _next);
	}
	

	public class ChoiceStart : BasicNode
	{
		public readonly string Text;

		public ChoiceStart(DialogueTree tree, string text) : base(tree)
		{ 
			Text = text;
		}

		public override IResponds ProcessReturnThisIfIsIRespondsElseNext() => NextNode.ProcessReturnThisIfIsIRespondsElseNext();
	}

	public class ChoiceEnd : BasicNode
	{
		public ChoiceEnd(DialogueTree tree) : base(tree)
		{ }

		public override IResponds ProcessReturnThisIfIsIRespondsElseNext() => NextNode.ProcessReturnThisIfIsIRespondsElseNext();
	}
}