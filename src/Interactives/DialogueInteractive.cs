using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DialogueInteractive : Interactive
	{
		[Signal] public delegate void OnFinished();

		[Export] private string _interactionText = "Speak";
		[Export(PropertyHint.File, "*.dial")] private string _dialoguePath = "";

		public override string GetLookAtText() => _interactionText;

		public override async Task Interact()
		{
			await LevelDialoguePlayer.PlayDialogue(_dialoguePath, this);
			
			EmitSignal(nameof(OnFinished));
		}
	}
}