using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	/// <summary>
	/// Represents the various damage dealing areas around the controller.
	/// </summary>
	public enum DamageDealerTargettingArea
	{
		Front,
		Count
	}

	/// <summary>
	/// Represents the various damage taking areas around the controller.
	/// </summary>
	public enum DamageTakerTargetArea
	{
		Body,
		Count
	}

	public static class DamageTakerAreaExtensions
	{
		// by god i hope this is all correct

		/// <summary>
		/// Searches through this node's scene tree and retrieves all IDamageTakers, mapping them to their respective dealer types.
		/// </summary>
		/// <returns></returns>
		public static EnumToCollectionMap<IDamageTakerDetector, DamageDealerTargettingArea> GetDealerTypeToDetectorCollection(this Node root)
		{
			return new EnumToCollectionMap<IDamageTakerDetector, DamageDealerTargettingArea>(root.GetAllProgenyNodesOfType<IDamageTakerDetector>(), d => d.DealerType);
		}

		public static EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea> GetDealerTypeToDealers(this Node root)
		{
			return new EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea>(root.GetAllProgenyNodesOfType<DamageDealer>(), dealer => dealer.DealerType);
		}
	}	
}
