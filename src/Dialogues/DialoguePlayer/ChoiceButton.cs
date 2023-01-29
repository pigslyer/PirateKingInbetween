using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class ChoiceButton : Container
	{
		#region Paths
		[Export] private NodePath __numberLabelPath = null;
		[Export] private NodePath __buttonPath = null;

		#endregion

		public void Initialize(uint number, string text)
		{
			GetNode<Label>(__numberLabelPath).Text = number.ToString();

			Button button = GetNode<Button>(__buttonPath);
			button.Text = text;

			if (number < 10)
			{
				ShortCut shortcut = new ShortCut();
				InputEventKey ev = new InputEventKey();

				ev.Scancode = (uint)KeyList.Key0 + number;

				shortcut.Shortcut = ev;
				button.Shortcut = shortcut;
			}
		}

		// cleaner than setting up a wrapping signal
		public Button GetButton() => GetNode<Button>(__buttonPath);
	}
}