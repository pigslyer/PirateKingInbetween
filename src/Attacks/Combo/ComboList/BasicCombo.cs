using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos.List
{
	// this contains the code for all the basic on ground slashes and their followups
	public abstract class BasicComboBase : Combo
	{
		private readonly float _comboLength;
		private readonly ReadOnlyCollection<(DamageAmount damage, ComboExecutorDamageDealers damageDealer, FloatInterval validRange)> _damageInstances;
		protected BasicComboBase(float length, params (DamageAmount damage, ComboExecutorDamageDealers damageDealer, FloatInterval valid)[] damageInstances)
		{ 
			_comboLength = length;
			_damageInstances = new ReadOnlyCollection<(DamageAmount damage, ComboExecutorDamageDealers damageDealer, FloatInterval validRange)>(damageInstances);
		}

		protected override async Task BeginCombo(IComboExecutor executor)
		{
			foreach (var instance in _damageInstances)
			{
				DealDamageFor(instance.validRange, instance.damageDealer, new DamageData(instance.damage, () => executor.GlobalPosition));
			}

			await WaitFor(_comboLength);
		}
	}


	[ComboAttr(
		inputs: new AttrInput[]{
			AttrInput.Melee
		},
		holdForward: UsageReq.Required,
		onFloor: UsageReq.Required
	)]
	public class BasicCombo1 : BasicComboBase
	{
		public BasicCombo1() : base(
			length: 0.6f,
			(1, ComboExecutorDamageDealers.Front, (0.3f, 0.5f))
		) { }
	}

	[ComboAttr(
		inputs: new AttrInput[]{
			AttrInput.Melee,
		},
		holdForward: UsageReq.Required,
		onFloor: UsageReq.Required,
		lastRequiredCombo: typeof(BasicCombo1),
		lowerTimeSinceLastCombo: 0f,
		higherTimeSinceLastCombo: 0.3f
	)]
	public class BasicCombo2 : BasicComboBase
	{
		public BasicCombo2() : base(
			length : 0.4f,
			(1, ComboExecutorDamageDealers.Front, (0.1f, 0.3f))
		) { }
	}

	[ComboAttr(
		inputs: new AttrInput[]{
			AttrInput.Melee,
		},
		holdForward: UsageReq.Required,
		onFloor: UsageReq.Required,
		lastRequiredCombo: typeof(BasicCombo2),
		lowerTimeSinceLastCombo: 0f,
		higherTimeSinceLastCombo: 0.5f
	)]
	public class BasicCombo3 : BasicComboBase
	{
		public BasicCombo3() : base(
			length : 1.5f,
			(3, ComboExecutorDamageDealers.Front, (0.5f, 0.8f))
		) { }
	}
}