using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween
{
	/// <summary>
	/// A text display that can appear and disappear on command.
	/// </summary>
	public interface ITextDisplay : ICanvasItem
	{
		void Appear(Node parent, Vector2 globalPosition, string text);
		void Disappear();
		string CurrentText();
	}
}