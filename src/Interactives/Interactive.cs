using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public abstract class Interactive : Area2D
	{
		public abstract Task Interact();
		public abstract string GetLookAtText();
	}
}