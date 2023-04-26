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
		public delegate void OnDamageTakenEventHandler(IDamageDealer source, DamageData data);

		public event OnDamageTakenEventHandler OnDamageTaken;
	}
}