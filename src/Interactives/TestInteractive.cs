using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class TestInteractive : Interactive
	{
		[Export] private string _displayText = "";

		public override string GetLookAtText() => _displayText;

		public override async Task Interact()
		{
			GD.Print($"looked at item with desc {_displayText}");

			await this.AwaitIdle();
		}
	}
}