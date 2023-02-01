using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Autoloads;
using PirateInBetween.Game.Dialogue;
using PirateInBetween.Game.Dialogue.Tree;

namespace PirateInBetween.Game
{
	public class LevelDialoguePlayer : Autoload<LevelDialoguePlayer>
	{
		private const string WORKING_DIRECTORY = "res://assets/dialogue/";

		[Export] private PackedScene _dialoguePlayerScene = null;

		private DialoguePlayer _dialoguePlayer;

		public override void _Ready()
		{
			base._Ready();

			_dialoguePlayer = _dialoguePlayerScene.Instance<DialoguePlayer>();
			AddChild(_dialoguePlayer);
		}

		public static async Task PlayDialogue(string path, Godot.Node source)
		{
			await Instance._dialoguePlayer.Play(DialogueTree.LoadFromFile(path, WORKING_DIRECTORY));
		}
	}	
}