using Godot;
using System;

namespace PirateInBetween.Game.Autoloads
{
	public abstract class Autoload<T> : Node where T : Autoload<T>
	{
		protected static T Instance { get; private set; }

		public override void _Ready()
		{
			Instance = (T)this;
		}
	}
}
