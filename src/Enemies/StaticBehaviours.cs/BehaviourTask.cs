using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Enemies.Behaviours
{
	public struct BehaviourTask
	{
		public readonly Action WhatDo;
		public readonly Func<bool> Condition;
	}
}