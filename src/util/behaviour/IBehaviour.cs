using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Behaviour
{
	public interface IBehaviour 
	{
		public void RunBehaviour();

		public BehaviourManager BehaviourManager { get; set; }
	}
}