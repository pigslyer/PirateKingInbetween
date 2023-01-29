using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
	public interface IProjectile<in T> : IProjectile
	{
		void SetData(DamageData damageData, T data);

	}

	public interface IProjectile
	{ 
		void Shoot(Vector2 startingPosition, MovingParent parent);
	}
}