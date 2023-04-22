using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.FrameData
{
	public interface IFrameData
	{
		public Vector2 Velocity { get; set; }
		public float Delta { get; }
		public bool IsOnFloor { get; }

		public CharacterAnimation CurrentAnimation {get; set; }
	}
}