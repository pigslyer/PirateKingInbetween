using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public abstract class PlayerBehaviour : Node
	{

		private PlayerController _player;

		public void Initialize(PlayerController player)
		{
			_player = player;
		}

		protected T GetSiblingBehaviour<T>(BehavioursPos behaviour) where T : PlayerBehaviour
		{
			return GetParent().GetChild<T>((int)behaviour);
		}

		protected bool IsOnFloor() => _player.IsOnFloor();
		protected PlayerController GetPlayer() => _player;

		protected void SetBehavioursEnabled(Behaviours behaviour, bool state)
		{
			if (state)
			{
				_player.ActiveBehaviours |= behaviour;				
			}
			else
			{
				_player.ActiveBehaviours &= ~behaviour;
			}
		}

		public abstract void Run(PlayerCurrentFrameData data);

		[Flags] public enum Behaviours
		{
			None = 0,
			HorizontalMovement = 1 << BehavioursPos.HorizontalMovement,
			Falling = 1 << BehavioursPos.Falling,
			Jumping = 1 << BehavioursPos.Jumping,
			Carrying = 1 << BehavioursPos.Carrying,
			MeleeAttack = 1 << BehavioursPos.MeleeAttack,
			RangedAttack = 1 << BehavioursPos.RangedAttack,
			Noclip = 1 << BehavioursPos.Noclip,
			AnimationSelector = 1 << BehavioursPos.AnimationSelector,
			Default = ~0 - Noclip,
		}

		// these have to be the same order as "States" in the player scene.
		// Behaviours is automatically changed to reflect changes in this enum, so long as new states are added to it
		protected enum BehavioursPos
		{
			HorizontalMovement,
			Falling,
			Jumping,
			Carrying,
			MeleeAttack,
			RangedAttack,
			Noclip,
			AnimationSelector,
		}
	}
}