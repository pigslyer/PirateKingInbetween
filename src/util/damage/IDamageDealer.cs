using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Damage
{
	public interface IDamageDealer
	{
		public void Enable(DamageData data);
		public void Disable();
		
		public PhysicsLayers2D TargetLayers { get; }
	}
}