using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public interface IHasCenterOfMass
	{
		public Vector2 CenterOfMass { get; }
	}
}