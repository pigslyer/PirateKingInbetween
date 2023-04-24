using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public class SingletonReal<Itself> where Itself : SingletonReal<Itself>, new()
	{
		private static Itself? _instance;
		protected static Itself Instance
		{
			get => _instance ??= new();
		}
	}
}