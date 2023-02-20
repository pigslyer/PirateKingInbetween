using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ComboAttr : Attribute, IComparable<ComboAttr>
	{
		public ReadOnlyCollection<ComboInputContainer> Inputs => new ReadOnlyCollection<ComboInputContainer>(_inputs);
		/// <summary>
		/// This combo can only trigger if this combo was the last one to have triggered.
		/// </summary>
		public readonly Type LastRequiredCombo;
		/// <summary>
		/// Whether <see cref="ComboInput.Forward"/> need be held for this combo to trigger.
		/// </summary>
		public readonly UsageReq MustHoldForward;
		/// <summary>
		/// Whether <see cref="IComboExecutor.IsOnFloor"/> must evaluate as true for the combo to trigger.
		/// </summary>
		public readonly UsageReq MustBeOnFloor;
		private readonly ReadOnlyCollection<ComboInputContainer> _inputs;
		public readonly FloatInterval IntervalSinceLastCombo;

		public ComboAttr(AttrInput[] inputs, UsageReq holdForward, UsageReq onFloor, Type lastRequiredCombo = null, float lowerTimeSinceLastCombo = 0f, float higherTimeSinceLastCombo = float.PositiveInfinity)
		{
			ComboInputContainer[]  mappedInputs = new ComboInputContainer[inputs.Length];

			for (int i = 0; i < mappedInputs.Length; i++)
			{
				mappedInputs[i] = new ComboInputContainer( 
					(ComboInput) (inputs[i] & ~AttrInput.TimingMask), 
					(ComboTiming) (inputs[i] & AttrInput.TimingMask)
				);
			}

			_inputs = new ReadOnlyCollection<ComboInputContainer>(mappedInputs);

			MustBeOnFloor = onFloor;
			MustHoldForward = holdForward;
			LastRequiredCombo = lastRequiredCombo;
			IntervalSinceLastCombo = new FloatInterval(lowerTimeSinceLastCombo, higherTimeSinceLastCombo);
		}

		public int CompareTo(ComboAttr o)
		{
			// one has it, one doesn't, put the one that does in front
			if ((LastRequiredCombo != null) != (o.LastRequiredCombo != null))
			{
				return LastRequiredCombo != null ? -1 : 1;
			}
			
			if ((IntervalSinceLastCombo != null) != (o.IntervalSinceLastCombo != null))
			{
				return IntervalSinceLastCombo != null ? -1 : 1;
			}

			if (MustBeOnFloor != o.MustBeOnFloor)
			{
				return MustBeOnFloor.CompareTo(o.MustBeOnFloor);
			}

			if (MustHoldForward != o.MustHoldForward)
			{
				return MustHoldForward.CompareTo(o.MustHoldForward);
			}

			if (IntervalSinceLastCombo != null && IntervalSinceLastCombo != o.IntervalSinceLastCombo)
			{
				// i can't concieve of a way to naturally order 2 float intervals so we're doing it like this.
				return IntervalSinceLastCombo.ToString().CompareTo(o.IntervalSinceLastCombo.ToString());
			}

			// both have it, we have to choose which to put in front
			// this has to be last in the case that: we have 2 followups to 1 attack, one demands the player move forward during, the other that he doesn't.
			// if this were before those checks, these 2 *different* followups would be evaluated as the same and throw a fit
			// if this comparison wasn't included then these 2 followsup would also be considered to be the same.
			if (LastRequiredCombo != null && LastRequiredCombo != o.LastRequiredCombo)
			{
				return LastRequiredCombo.Name.CompareTo(o.LastRequiredCombo.Name);
			}

			return 0;
		}

		public override string ToString()
		{
			return $"On floor: {MustBeOnFloor}, Hold forward: {MustHoldForward}, Interval since last combo: {IntervalSinceLastCombo}, Last required combo: {LastRequiredCombo}";
		}
	}

	// workaround because objects (and their arrays) can't be parameters for attributes
	// this enum must be kept in sync with both ComboInput and ComboTiming

	/// <summary>
	/// All inputs default to <see cref="ComboTiming.Short"/> speed.
	/// </summary>
	public enum AttrInput
	{
		Forward,
		SwitchDirection,
		Up,
		Down,
		Melee,
		Ranged,

		Dodge = 1 << 29,
		TimingMask = 3 << 30,
		Short = 0,
		Medium = 1 << 30,
		Long = 1 << 31,
		// can also have something saved with 3 << 30
	}
}