using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Damage
{
	public record DamageData(float DamageAmount, float StunTime, float Knockback, PhysicsLayers2D TargetLayers)
	{
		public DamageData(PhysicsLayers2D TargetLayers) : this(0, 0, 0, TargetLayers)
		{ }
	}
}