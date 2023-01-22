using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class DialogueCharacter
	{
		public readonly Texture CharacterPortrait;
		public readonly string Name;

		private static int counter = 0;
		private DialogueCharacter()
		{
			Name = $"Placeholder:{counter++}";
			CharacterPortrait = null;
		}
		
		public static DialogueCharacter Load(string file)
		{
			throw new NotImplementedException();
		}

		public static DialogueCharacter Load()
		{
			return new DialogueCharacter();
		}
	}
}