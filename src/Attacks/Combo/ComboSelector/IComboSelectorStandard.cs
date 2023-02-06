using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos.Tree
{
	/// <summary>
	/// This standard only checks if the given combo selector fulfills the stnadards laid out by the ComboAttr
	/// </summary>
	public interface IComboSelectorStandard
	{
		bool CanUseCombo(ComboAttr attr, Combo combo);

		void UsingCombo(Combo combo);
	}
}