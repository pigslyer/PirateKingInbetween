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

		public const UsageReq BASIC_COMBO_MOVEMENT_REQUIRED = UsageReq.Optional;

		private readonly float _comboLength;
		private readonly float _velocity;
		private readonly Animations _slashAnim;
		private readonly ReadOnlyCollection<DamageInstance> _damageInstances;
		protected BasicComboBase(float length, float velocity, Animations slashAnim, params DamageInstance[] damageInstances)
		{ 
			_comboLength = length; _velocity = velocity; _slashAnim = slashAnim;
			_damageInstances = new ReadOnlyCollection<DamageInstance>(damageInstances);
		}

		protected override void BeginCombo()
		{

			foreach (var instance in _damageInstances)
			{
				AddTask().DealDamageFor(instance);
			}

			AddTask().DoFor(_comboLength, (a,b,c) => CurrentData.ResetHorizontal());

			AddTask().PlayFor(_slashAnim, _comboLength);

			//EnableHorizontalControl(_comboLength, 10f, _velocity);
		}
	}


	[ComboAttr(
		inputs: new AttrInput[]{
			AttrInput.Melee
		},
		holdForward: BasicComboBase.BASIC_COMBO_MOVEMENT_REQUIRED,
		onFloor: UsageReq.Required
	)]
	public class BasicCombo1 : BasicComboBase
	{
		public BasicCombo1() : base(
			length: 0.6f,
			velocity: 20f,
			slashAnim: Animations.BasicCombo1,
			new DamageInstance(
				new DamageAmount(
					amount: 1, 
					stun: 0.65f
				),
				(0.2f, 0.5f),
				DamageDealerTargettingArea.Front
			)
		) { }
	}

	[ComboAttr(
		inputs: new AttrInput[]{
			AttrInput.Melee,
		},
		holdForward: BasicComboBase.BASIC_COMBO_MOVEMENT_REQUIRED,
		onFloor: UsageReq.Required,
		lastRequiredCombo: typeof(BasicCombo1),
		lowerTimeSinceLastCombo: 0f,
		higherTimeSinceLastCombo: 0.5f
	)]
	public class BasicCombo2 : BasicComboBase
	{
		public BasicCombo2() : base(
			length : 0.4f,
			velocity: 5f,
			slashAnim: Animations.BasicCombo2,
			new DamageInstance(
				new DamageAmount(
					amount: 1,
					stun: 0.55f
				), 
				(0.1f, 0.3f), 
				DamageDealerTargettingArea.Front
			)
		) { }
	}

	[ComboAttr(
		inputs: new AttrInput[]{
			AttrInput.Melee,
		},
		holdForward: BasicComboBase.BASIC_COMBO_MOVEMENT_REQUIRED,
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
			slashAnim: Animations.BasicCombo3,
			new DamageInstance(
				new DamageAmount(
					amount: 3, 
					stun: 0.7f
				),
				(0.15f, 0.8f),
				DamageDealerTargettingArea.Front,
				() => 60f.ToVectorDeg(3)
			)
		) { }
	}
}