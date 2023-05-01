using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Damage
{
	public partial class DamageTakerArea : Area2DOverride, IDamageTaker
	{
		[Export] private PhysicsLayers2D _occupiesLayer = PhysicsLayers2D.None;
		private bool _isEnabled = false;

		private event Action<DamageData>? _onDamageTaken;
		public event Action<DamageData> OnDamageTaken
		{
			add => _onDamageTaken += value;
			remove => _onDamageTaken -= value;
		}

		public void Enable()
		{
			_isEnabled = true;
		}

		public void Disable()
		{
			_isEnabled = false;
		}

		public void Hit(DamageData data)
		{
			_onDamageTaken?.Invoke(data);
		}
	}
}