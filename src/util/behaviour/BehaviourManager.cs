using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Behaviour
{
	public class BehaviourManager
	{
		private List<IBehaviour> _existingBehaviours;
		
		public IReadOnlyList<IBehaviour> Behaviours => _existingBehaviours;

		private BehaviourManager(IList<IBehaviour> existingBehaviours)
		{ 
			_existingBehaviours = new(existingBehaviours);

			foreach (IBehaviour b in existingBehaviours)
			{
				b.BehaviourManager = this;
			}
		}

		public void RunBehaviours()
		{
			foreach (IBehaviour b in _existingBehaviours)
			{
				b.RunBehaviour();
			}
		}


		public static BehaviourManager Generate(IList<IBehaviour> startingBehaviours)
		{
			return new(startingBehaviours);
		}
	}
}