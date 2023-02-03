using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	public static class LocalExtensions
	{
		public static Vector2 FaceForward(this float deg, ICombatFrameData data) => FaceForward(deg.ToVectorDeg(), data);
		public static Vector2 FaceBackward(this float deg, ICombatFrameData data) => FaceBackward(deg.ToVectorDeg(), data);


		public static Vector2 FaceForward(this Vector2 vec, ICombatFrameData data)
		{
			return new Vector2(vec.x * (data.FacingRight ? 1 : -1), vec.y);
		}

		public static Vector2 FaceBackward(this Vector2 vec, ICombatFrameData data)
		{
			return new Vector2(vec.x * (data.FacingRight ? -1 : 1), vec.y);
		}

		public static float PowerOf4(this float val) => val * val * val * val;
	}
}