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

		public async Task AwaitContinue()
		{
			await ToSignal(AddButton("(Continue)"), "pressed");
			RemoveButtons();
		}

		public async Task<int> SelectChoice(string[] choices)
		{
			async Task<int> WaitForButton(Button button, int value)
			{
				await ToSignal(button, "pressed");
				return value;
			}

			Task<int>[] tasks = new Task<int>[choices.Length];

			for (int i = 0; i < choices.Length; i++)
			{
				tasks[i] = WaitForButton(AddButton(choices[i]), i);
			}

			int ret = (await Task.WhenAny<int>(tasks)).Result;

			RemoveButtons();

			return ret;
		}

		private Button AddButton(string text)
		{
			Button button = _choiceButton.Instance<Button>();
			button.Text = text;
			button.AddToGroup(BUTTON_GROUP);
			_container.AddChild(button);
			return button;
		}

		private void RemoveButtons()
		{
			GetTree().CallGroup(BUTTON_GROUP, "queue_free");
		}
	}
}