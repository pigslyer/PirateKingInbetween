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
	public class ComboAttr : Attribute
	{
		public ReadOnlyCollection<ComboInputContainer> Inputs => new ReadOnlyCollection<ComboInputContainer>(_inputs);
		private readonly ComboInputContainer[] _inputs;

		public ComboAttr(AttrInput[] inputs)
		{
			_inputs = new ComboInputContainer[inputs.Length];

			for (int i = 0; i < _inputs.Length; i++)
			{
				_inputs[i] = new ComboInputContainer( 
					(ComboInput) (inputs[i] & ~AttrInput.TimingMask), 
					(ComboTiming) (inputs[i] & AttrInput.TimingMask)
				);
			}
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

		TimingMask = 3 << 30,
		Short = 0,
		Medium = 1 << 30,
		Long = 1 << 31,
		// can also have something saved with 3 << 31
	}
}