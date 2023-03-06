using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;

namespace PirateInBetween.Game.Enemies
{
	public abstract class EnemyModel : Node2D
	{
		#region Paths

		#endregion


		public void Initialize(EnemyController controller)
		{
			GenerateDamageDealersArray(controller); GenerateDamageTakersArray(controller);
		}

		#region Damage


		private EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea> _damageDealers;

		private EnumToCollectionMap<DamageTaker, DamageTakerTargetArea> _damageTakers;

		private void GenerateDamageDealersArray(EnemyController root)
		{
			_damageDealers = new EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea>(root.GetAllProgenyNodesOfType<DamageDealer>(), d => d.DealerType);
		}

		private void GenerateDamageTakersArray(EnemyController root)
		{
			_damageTakers = new EnumToCollectionMap<DamageTaker, DamageTakerTargetArea>(root.GetAllProgenyNodesOfType<DamageTaker>(), t => t.TakerType);
		}


		public void DealDamage(DamageDealerTargettingArea damageDealer, DamageData data) => _damageDealers.DoFor(d => d.Enable(data), damageDealer);

		public void StopDealingDamage(DamageDealerTargettingArea damageDealer) => _damageDealers.DoFor(d => d.Disable(), damageDealer);

		public void TakeDamage(DamageTakerTargetArea damageTaker) => _damageTakers.DoFor(t => t.Enable(), damageTaker);
		public void StopTakingDamage(DamageTakerTargetArea damageTaker) => _damageTakers.DoFor(t => t.Disable(), damageTaker);

		#endregion
	
		public abstract void PlayAnimation(Animations animation);
	}
}