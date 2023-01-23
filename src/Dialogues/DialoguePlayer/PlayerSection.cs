using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class PlayerSection : Panel
	{
		private const string BUTTON_GROUP = "PLAYER_SECTION_BUTTONS";


		[Export] private PackedScene _choiceButton;

		#region Paths
		[Export] private NodePath _choicesVBoxPath;
		[Export] private NodePath _speakerNamePath;

		#endregion

		private Container _container;

		public override void _Ready()
		{
			base._Ready();

			_container = GetNode<Container>(_choicesVBoxPath);

		}

		public async Task AwaitContinue(string text = "(Continue)")
		{
			await ToSignal(AddButton(1, text), "pressed");
			RemoveButtons();
		}

		public async Task<uint> SelectChoice(string[] choices, string speaker)
		{
			async Task<uint> WaitForButton(Button button, uint value)
			{
				await ToSignal(button, "pressed");
				return value;
			}

			Label speakerLabel = GetNode<Label>(_speakerNamePath);
			speakerLabel.Text = speaker;

			Task<uint>[] tasks = new Task<uint>[choices.Length];

			for (uint i = 0; i < choices.Length; i++)
			{
				tasks[i] = WaitForButton(AddButton(i + 1, choices[i]), i);
			}

			uint ret = (await Task.WhenAny<uint>(tasks)).Result;

			RemoveButtons();
			speakerLabel.Text = " ";

			return ret;
		}

		private Button AddButton(uint number, string text)
		{
			ChoiceButton button = _choiceButton.Instance<ChoiceButton>();
			button.Initialize(number, text);
			button.AddToGroup(BUTTON_GROUP);
			_container.AddChild(button);
			return button.GetButton();
		}

		private void RemoveButtons()
		{
			GetTree().CallGroup(BUTTON_GROUP, "queue_free");
		}
	}
}