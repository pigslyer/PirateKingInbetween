using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public interface IDamageTakerDetector
	{
		DamageDealerTargettingArea DealerType { get; }

		bool CanSeeTaker();
		DamageTakerTargetArea GetTakerArea();

	}	
}