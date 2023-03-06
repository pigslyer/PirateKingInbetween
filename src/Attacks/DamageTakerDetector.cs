using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public class DamageTakerDetector : Area2D, IDamageTakerDetector
	{
		[Export] public DamageDealerTargettingArea DealerType { get; private set; }

		public bool CanSeeTaker() => GetOverlappingAreas().Count > 0;

		public DamageTakerTargetArea GetTakerArea() => ((DamageTaker)(GetOverlappingAreas()[0])).TakerType;
	}
}