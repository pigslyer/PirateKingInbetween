using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public partial class Singleton<Itself> : Node where Itself : Singleton<Itself>
	{
		protected static Itself Instance { get; private set; } = null!;

		public override void _Ready()
		{
			base._Ready();

			Instance = (Itself)this;
		}
	}
}