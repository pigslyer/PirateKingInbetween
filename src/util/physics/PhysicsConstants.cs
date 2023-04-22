using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Physics
{
	public enum PhysicsLayers2D : uint
	{
		World = 0,
		Prop = 1,
		Player = 2,
		Enemy = 4,
		Interactive = 8,
	}
}
