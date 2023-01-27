using Godot;
using System;

namespace PirateInBetween.Game.Player
{
	public abstract class PlayerBehaviour : Node
	{

		private PlayerController _player;
		private PlayerBehaviourManager _state;

		public void Initialize(PlayerController player, PlayerBehaviourManager state)
		{
			_player = player; _state = state;
		}

		protected T GetSiblingBehaviour<T>(BehavioursPos behaviour) where T : PlayerBehaviour
		{
			return GetParent().GetChild<T>((int)behaviour);
		}

		protected Vector2 Position => _player.Position;
		protected Vector2 GlobalPosition => _player.GlobalPosition;
		
		
		protected bool IsOnFloor() => _state.IsPlayerOnFloor();

		protected PlayerController GetPlayer() => _player;


		protected bool CanChangeActive { get => _state.CanChangeActive(this); }

		/// <summary>
		/// If <see cref="CanChangeActive"/> is true, allows the calling behaviour to set whether
		/// other behaviours can control active behaviours.
		/// </summary>
		/// <param name="state">True if this behaviour should take control, false otherwise.</param>
		protected void SetBehaviourChangesDisabled(bool state)
		{
			if (!CanChangeActive)
			{
				GD.PushWarning($"State {Name} can't change behaviour stopper state because behaviour stopper is on and it isn't in control.");
				return;
			}

			if (state)
			{
				_state.SetStoppingBehaviourActive(this);
			}
			else
			{
				_state.SetStoppingBehaviourActive(null);
			}
		}

		/// <summary>
		/// If <see cref="CanChangeActive"/> is true, allows the calling behaviour to change which states are active
		/// and which aren't.
		/// </summary>
		/// <param name="targetBehaviours">The behaviours to be changed.</param>
		/// <param name="state">Whether they should be turned on or off.</param>
		protected void SetBehavioursEnabled(Behaviours targetBehaviours, bool state)
		{
			if (!CanChangeActive)
			{
				GD.PushWarning("Attempted to change active behaviours when this state isn't allowed to.");
			}
			
			if (state)
			{
				_state.ActiveBehaviours |= targetBehaviours;
			}
			else
			{
				_state.ActiveBehaviours &= ~targetBehaviours;
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

			/// <summary>
			/// These are all behaviours which the player has enabled at the start.
			/// Enabled behaviours need not do anything even if they're enabled, they can just check for inputs.
			/// </summary>
			/// <returns></returns>
			Default = ~Behaviours.Noclip,
			
			BasicMovementNoJump = Behaviours.HorizontalMovement | Behaviours.Falling,
			BasicMovement = Behaviours.HorizontalMovement | Behaviours.Falling | Behaviours.Jumping,
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