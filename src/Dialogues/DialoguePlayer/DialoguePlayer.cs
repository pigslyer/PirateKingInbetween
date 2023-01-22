using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class DialoguePlayer : Popup
	{
		[Export] private PackedScene _textDisplayScene;

		#region Paths
		[Export] private NodePath __portraitTexturePath = null;
		[Export] private NodePath __playerSectionPath = null;
		[Export] private NodePath __lineDisplayPath = null;

		#endregion

		private Container _lineDisplay;

		public override void _Ready()
		{
			base._Ready();

			_lineDisplay = GetNode<Container>(__lineDisplayPath);
		}

		public async Task Play(Dialogue dialogue)
		{
			Popup_();

			PlayerSection player = GetNode<PlayerSection>(__playerSectionPath);

			int choice = 0;
			DialogueResponse next;


			while (dialogue.HasNext())
			{

				if (dialogue.RequiresChoice())
				{
					next = dialogue.Choose(choice);
				}
				else
				{
					next = dialogue.Next();

				}
				
				switch (next.Type)
				{
					case DialogueResponse.Types.End:
						DialogueLine.FreeAll();
						Hide();

						break;

					case DialogueResponse.Types.Line:
						await AddLine(next.Line, next.Speaker.Name, !next.IsPlayer);
						await player.AwaitContinue();

						break;

					case DialogueResponse.Types.Choice:
						choice = await player.SelectChoice(next.Choices);

						await AddLine(next.Choices[choice], next.Speaker.Name, false);
						break;
				}
			}
		}


		private async Task AddLine(string text, string speaker, bool onRight)
		{
			DialogueLine line = _textDisplayScene.Instance<DialogueLine>();	
			line.Initialize(speaker, text, onRight);
			_lineDisplay.AddChildBelowNode(_lineDisplay.GetChild(_lineDisplay.GetChildCount()-2), line);

			await this.WaitFor(0.2f);
		}
	}
}