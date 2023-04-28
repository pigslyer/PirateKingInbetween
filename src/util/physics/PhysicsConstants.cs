using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Physics
{
	[Flags] public enum PhysicsLayers2D : uint
	{
		None = 0,
		World = 1,
		Player = 2,
		Enemy = 4,
		Interactive = 8,
		// all IDamageDealer and IDamageData implementations must occupy this layer, however they can use the other layers internally to tell apart targets
		Damage = 16,
	}
}
