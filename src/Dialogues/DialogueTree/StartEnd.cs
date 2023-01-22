

namespace PirateInBetween.Game.Dialogue.Tree
{
	public class Start : BasicNode, IResponds
	{
		public Start(DialogueTree tree) : base(tree)
		{ }
		
		public override IResponds ProcessReturnThisIfIsIRespondsElseNext() => NextNode.ProcessReturnThisIfIsIRespondsElseNext();

		public IResponds GetNext() => NextNode.ProcessReturnThisIfIsIRespondsElseNext();
		public DialogueResponse Response() => null;
	}

	public class End : Node, IResponds
	{
		public End(DialogueTree tree) : base(tree)
		{ }
		
		public override IResponds ProcessReturnThisIfIsIRespondsElseNext() => this;

		public IResponds GetNext() => this;

		public DialogueResponse Response()
		{
			return new DialogueResponse();
		}
	}
}