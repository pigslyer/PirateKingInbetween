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
		public Types Type { get; private set; }

		public DialogueCharacter Speaker { get; private set; }

		public string Line {get; private set;}
		public bool IsPlayer { get; private set; }
		public string[] Choices { get; private set; } = null;
		public int ChoiceCount => Choices?.Length ?? -1;

		public DialogueResponse()
		{
			Type = Types.End;
		}

		public DialogueResponse(DialogueCharacter speaker, string line, bool isPlayer)
		{
			Type = Types.Line;
			Speaker = speaker; Line = line; IsPlayer = isPlayer;
		}

		public DialogueResponse(DialogueCharacter speaker, string[] choices)
		{
			Type =Types.Choice;
			Speaker = speaker; Choices = choices;
		}

		public override string ToString()
		{
			return $"Type: {Type}, Speaker: {Speaker}, Line: {Line}, IsPlayer: {IsPlayer}, ChoiceCount: {ChoiceCount}";
		}

		public enum Types
		{
			End,
			Line,
			Choice,
		}
	}
}