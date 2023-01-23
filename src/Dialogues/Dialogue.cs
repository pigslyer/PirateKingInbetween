using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public abstract class Dialogue
	{
		public IDialogueResourceLoader Loader {get; private set; }

		public Dialogue(IDialogueResourceLoader loader)
		{
			Loader = loader;
		}

		/// <summary>
		/// Returns the next <see cref="DialogueResponse"/>.
		/// Choice is <see langword="null"/> if the previous line wasn't a choice, otherwise it falls in the interval
		///  [0-choiceCount).
		/// </summary>
		/// <param name="choice"></param>
		/// <returns></returns>
		protected abstract DialogueResponse GetNextResponse(uint? choice = null);

		/// <summary>
		/// The last <see cref="DialogueResponse"/> given by either <see cref="Next"/> or <see cref="Choose"/>.
		/// </summary>
		/// <value></value>
		public DialogueResponse LastResponse {get; private set; } = null;

		/// <summary>
		/// Returns <see langword="true"/> if this <see cref="Dialogue"/> has lines left.
		/// </summary>
		/// <returns></returns>
		public bool HasNext() => LastResponse == null || LastResponse.Type != DialogueResponse.Types.End;

		/// <summary>
		/// Returns <see langword="true"/> if this <see cref="Dialogue"/> last gave a <see cref="DialogueResponse.Types.Choice"/> response
		/// and <see cref="HasNext"/> is <see langword="true"/>, thereby expecting a call to <see cref="Choose"/> instead of <see cref="Next"/>.
		/// </summary>
		/// <returns></returns>
		public bool RequiresChoice() => HasNext() && LastResponse != null && LastResponse.Type == DialogueResponse.Types.Choice;


		/// <summary>
		/// Returns the next <see cref="DialogueResponse"/>. If <see cref="HasNext"/> is false or <see cref="RequiresChoice"/> 
		/// returns <see langword="true"/>, throws an <see cref="InvalidOperationException"/> instead.
		/// </summary>
		/// <returns></returns>
		public DialogueResponse Next()
		{
			if (!HasNext() || RequiresChoice())
			{
				throw new InvalidOperationException();
			}

			LastResponse = GetNextResponse();
			return LastResponse;
		}

		/// <summary>
		/// Returns a response based on the given choice. If <see cref="HasNext"/> or <see cref="RequiresChoice"/> is <see langword="false"/>,
		/// throws an <see cref="InvalidOperationException"/> instead, or a <see cref="IndexOutOfRangeException"/> if the
		/// response is outside the bounds given by <see cref="GetLast"/>.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		public DialogueResponse Choose(uint response)
		{
			if (!HasNext() || !RequiresChoice())
			{
				throw new InvalidOperationException();
			}

			if (!(response >= 0 && response < LastResponse.ChoiceCount))
			{
				throw new IndexOutOfRangeException();
			}

			return LastResponse = GetNextResponse(response);
		}
	}
}