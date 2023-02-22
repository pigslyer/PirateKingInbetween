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

		#region Damage paths

		[Export] private NodePath __frontDealerPath = null;
		[Export] private NodePath __bodyTakerPath = null;

		#endregion

		private DamageDealer[] _damageDealers;
		private CollisionShape2D[] _damageTakers;

		public override void _Ready()
		{
			base._Ready();

			GenerateDamageDealersArray();
			GenerateDamageTakersArray();
		}

		#region Damage
		private void GenerateDamageDealersArray()
		{
			_damageDealers = new DamageDealer[(int)ComboExecutorDamageDealers.Count];

			// add more as time goes on
			_damageDealers[(int)ComboExecutorDamageDealers.Front] = GetNodeOrNull<DamageDealer>(__frontDealerPath);
		}

		private void GenerateDamageTakersArray()
		{
			_damageTakers = new CollisionShape2D[(int)ComboExecutorDamageTaker.Count];

			// add more shit as time goes on
			_damageTakers[(int)ComboExecutorDamageTaker.Body] = GetNodeOrNull<CollisionShape2D>(__bodyTakerPath);
		}

		private DamageDealer GetDealer(ComboExecutorDamageDealers dealer) => _damageDealers[(int)dealer] ?? throw new NotImplementedException($"Model does not contain reference to {dealer}.");
		private CollisionShape2D GetTaker(ComboExecutorDamageTaker taker) => _damageTakers[(int)taker] ?? throw new NotImplementedException($"Model does not contain reference to {taker}.");


		public void DealDamage(ComboExecutorDamageDealers damageDealer, DamageData data) => GetDealer(damageDealer).Enable(data);

		public void StopDealingDamage(ComboExecutorDamageDealers damageDealer) => GetDealer(damageDealer).Disable();

		public void TakeDamage(ComboExecutorDamageTaker damageTaker) => GetTaker(damageTaker).Disabled = false;
		public void StopTakingDamage(ComboExecutorDamageTaker damageTaker) => GetTaker(damageTaker).Disabled = true;

		#endregion
	
		public abstract void PlayAnimation(Animations animation);
	}
}