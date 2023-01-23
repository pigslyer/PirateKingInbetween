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
		public Texture CharacterPortrait { get; protected set; }
		public string Name {get; protected set; }
	}
}