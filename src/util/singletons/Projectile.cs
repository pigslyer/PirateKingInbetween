using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Singletons
{
	public partial class Projectile : Singleton<Projectile>
	{
		public static void AddChild(Node child) => Instance.AddChild(child, forceReadableName: false);
	}
}