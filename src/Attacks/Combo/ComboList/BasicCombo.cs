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
		private readonly float _velocity;
		private readonly ReadOnlyCollection<(DamageAmount damage, ComboExecutorDamageDealers damageDealer, FloatInterval validRange)> _damageInstances;
		protected BasicComboBase(float length, float velocity, params (DamageAmount damage, ComboExecutorDamageDealers damageDealer, FloatInterval valid)[] damageInstances)
		{ 
			_comboLength = length; _velocity = velocity;
			_damageInstances = new ReadOnlyCollection<(DamageAmount damage, ComboExecutorDamageDealers damageDealer, FloatInterval validRange)>(damageInstances);
		}

		protected override void BeginCombo()
		{
			foreach (var instance in _damageInstances)
			{
				AddTask().DealDamageFor(instance.validRange, instance.damageDealer, instance.damage);
			}

			AddTask().DoFor(
				_comboLength,
				(float elapsed, float delta, float total) =>
				{
					if (CurrentData.IsMoving())
					{
						if (CurrentData.IsGoingBackwards())
						{
							CurrentData.SwitchDirection();
						}

						CurrentData.Velocity = 0f.FaceForward(CurrentData) * _velocity;
					}
					else
					{
						CurrentData.Velocity = Vector2.Zero;
					}
				}
			);

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
			velocity: 20f,
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
			velocity: 5f,
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
			velocity: 2f,
			(3, ComboExecutorDamageDealers.Front, (0.5f, 0.8f))
		) { }
	}
}