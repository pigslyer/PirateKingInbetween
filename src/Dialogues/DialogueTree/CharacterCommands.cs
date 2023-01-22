

namespace PirateInBetween.Game.Dialogue.Tree
{
	public class AddCharacter : BasicNode
	{
		public readonly string CharacterName;
		public readonly DialogueCharacter CharacterToAdd;

		public AddCharacter(DialogueTree tree, string characterName, DialogueCharacter characterToAdd) : base(tree)
		{
			CharacterName = characterName; CharacterToAdd = characterToAdd;
		}

		public override IResponds ProcessReturnThisIfIsIRespondsElseNext()
		{
			DialogueTree.AddCharacter(CharacterName, CharacterToAdd);	
			return NextNode.ProcessReturnThisIfIsIRespondsElseNext();
		}
	}
}