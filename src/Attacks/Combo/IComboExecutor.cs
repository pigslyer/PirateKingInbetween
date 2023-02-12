using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	/// <summary>
	/// Represents the character controller that's currently executing the combo.
	/// Some things (such as <see cref="IComboExecutor.CameraPosition"/>) are only applicable to the Player, therefore they should do nothing for other controllers.
	/// </summary>
	public interface IComboExecutor
	{
		Vector2 GlobalPosition { get; set; }
		Vector2 CameraPosition { get; set; }

		bool IsOnFloor { get; }

		void DealDamage(ComboExecutorDamageDealers damageDealer, DamageData data);
		void StopDealingDamage(ComboExecutorDamageDealers damageDealer);

		void TakeDamage(ComboExecutorDamageTaker to);
		void StopTakingDamage(ComboExecutorDamageTaker to);

		void OnDamageTakenSet(Combo.OnHitReaction damageTaken);
		Combo.OnHitReaction OnDamageTakenDefault();
	}

	/// <summary>
	/// Represents the various damage dealing areas around the controller.
	/// </summary>
	public enum ComboExecutorDamageDealers
	{
		Front,
	}

	/// <summary>
	/// Represents the various damage taking areas around the controller.
	/// </summary>
	public enum ComboExecutorDamageTaker
	{
		Body,
	}
}