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

		public static bool IsJustGoingForward(this ICombatFrameData data)
		{
			int diff = (InputManager.IsActionJustPressed(InputButton.MoveRight) ? 1 : 0) - (InputManager.IsActionJustPressed(InputButton.MoveLeft) ? 1 : 0);

			if (diff != 0)
			{
				return (diff == 1) == data.FacingRight;
			}

			return false;
		}
		
		public static bool IsGoingBackwards(this ICombatFrameData data)
		{
			if (data is Player.PlayerCurrentFrameData playerData)
			{
				return (playerData.Input.x < 0f) == data.FacingRight;
			}

			return (data.Velocity.x < 0f) == data.FacingRight;
		}

		public static bool IsGoingForward(this ICombatFrameData data)
		{
			if (data is Player.PlayerCurrentFrameData playerData)
			{
				return (playerData.Input.x > 0f) == playerData.FacingRight; 
			}

			return (data.Velocity.x > 0f) == data.FacingRight;
		}

		public static bool IsMoving(this ICombatFrameData data)
		{
			if (data is Player.PlayerCurrentFrameData playerData)
			{
				return playerData.Input.x != 0f;
			}

			return data.Velocity.x != 0f;
		}

	}
}