using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Damage
{
	public partial class DamageDealerArea : Area2DOverride
	{
		[Export] private PhysicsLayers2D _targetLayers = PhysicsLayers2D.None;
		public PhysicsLayers2D TargetLayers => _targetLayers;

		private DamageData? _currentData = null;


		public void Enable(DamageData data)
		{
			_currentData = data;
		}

		public void Disable()
		{
			_currentData = null;
		}

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			if (_currentData != null) foreach (Area2D area in GetOverlappingAreas())
			{
				if (area is IDamageTaker taker)
				{
					taker.Hit(_currentData);
				}
			}
		}
	}
}