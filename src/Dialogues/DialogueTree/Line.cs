

namespace PirateInBetween.Game.Dialogue.Tree
{
	public class CharacterLine : BasicNode, IResponds
	{
		public readonly string Line;
		public readonly string Character;

		public CharacterLine(DialogueTree tree, string line, string character) : base(tree)
		{
			Line = line; Character = character;
		}

		public override IResponds ProcessReturnThisIfIsIRespondsElseNext() => this;

		public DialogueResponse Response()
		{
			return new DialogueResponse(DialogueTree.GetCharacter(Character), Line, Character == DialogueTree.PLAYER_NAME);
		}

		public override string ToString() => $"{Character} {Line}";

		public IResponds GetNext() => NextNode.ProcessReturnThisIfIsIRespondsElseNext();
	}

}