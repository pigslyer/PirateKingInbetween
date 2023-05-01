using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Damage
{
	public interface IDamageTaker
	{
		public event Action<DamageData> OnDamageTaken;

		public void Enable();
		public void Disable();

		public void Hit(DamageData data);
	}
}