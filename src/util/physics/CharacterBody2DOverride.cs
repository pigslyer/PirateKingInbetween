using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Physics
{
	public partial class CharacterBody2DOverride : CharacterBody2D
	{
		public new PhysicsLayers2D CollisionLayer
		{
			get => (PhysicsLayers2D) base.CollisionLayer;
			set => base.CollisionLayer = (uint) value;
		}

		public new PhysicsLayers2D CollisionMask
		{
			get => (PhysicsLayers2D) base.CollisionMask;
			set => base.CollisionMask = (uint) value;
		}
	}
}