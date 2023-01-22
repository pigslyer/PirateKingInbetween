using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class DialogueResponse
	{
		public DialogueResponse()
		{
			Type = Types.End;
		}

		public DialogueResponse(string line)
		{
			Type = Types.Line;
			Line = line;
		}

		public DialogueResponse(string[] choices)
		{
			Type =Types.Choice;
			Choices = choices;
		}
		

		public readonly Types Type;
		public readonly string Line;
		public readonly string[] Choices;
		public int ChoiceCount => Choices.Length;

		public enum Types
		{
			End,
			Line,
			Choice,
		}
	}
}