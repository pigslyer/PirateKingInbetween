using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class DialoguePlayer : Control
	{
		[Export] private float _resetPositionTime = 0.2f;
		[Export] private PackedScene _textDisplayScene = null;

		#region Paths
		[Export] private NodePath __portraitTexturePath = null;
		[Export] private NodePath __playerSectionPath = null;
		[Export] private NodePath __lineDisplayPath = null;
		[Export] private NodePath __scrollContainerPath = null;

		#endregion

		private Container _lineDisplay;
		private ScrollContainer _scrollContainer;
		private PlayerSection _player;

		public override void _Ready()
		{
			base._Ready();
			
			Hide();

			_lineDisplay = GetNode<Container>(__lineDisplayPath);
			_scrollContainer = GetNode<ScrollContainer>(__scrollContainerPath);
			_player = GetNode<PlayerSection>(__playerSectionPath);
		}

		public async Task Play(Dialogue dialogue)
		{
			Show();

			uint choice = 0;
			DialogueResponse next;

			TextureRect portraitTexture = GetNode<TextureRect>(__portraitTexturePath);

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

			
				portraitTexture.Texture = next.Speaker?.CharacterPortrait;
				
				switch (next.Type)
				{
					case DialogueResponse.Types.End:
						await _player.AwaitContinue("(End)");

						DialogueLine.FreeAll(GetTree());
						Hide();

						break;

					case DialogueResponse.Types.Line:
						await AddLine(next.Line, next.Speaker.Name, !next.IsPlayer);
						await _player.AwaitContinue();

						break;

					case DialogueResponse.Types.Choice:
						choice = await _player.SelectChoice(next.Choices, next.Speaker.Name);

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

			await this.AwaitIdle();

			var tween = CreateTween();
			tween.TweenProperty(_scrollContainer, "scroll_vertical", Mathf.RoundToInt(_lineDisplay.RectSize.y - _scrollContainer.RectSize.y), _resetPositionTime);
			await this.WaitFor(_resetPositionTime);

			await this.WaitFor(0.2f);
		}
	}
}