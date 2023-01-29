using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player.Behaviours
{
	public class PlayerInteraction : PlayerBehaviour
	{
		#region Paths

		[Export] private NodePath __interactionDetectionAreaPath = null;

		#endregion

		private Godot.Area2D _interactionDetectionArea;
		private PlayerAnimationSelector _animSelector;

		private const Behaviours DISABLED_DURING_INTERACTION = Behaviours.Default & ~(Behaviours.Interaction);

		public override void _Ready()
		{
			base._Ready();

            _interactionDetectionArea = GetNode<Godot.Area2D>(__interactionDetectionAreaPath);
			_animSelector = GetSiblingBehaviour<PlayerAnimationSelector>(BehavioursPos.AnimationSelector);
		}

		public override void Run(PlayerCurrentFrameData data)
		{
			var array = _interactionDetectionArea.GetAreas();

			if (!data.IsBusy && CanChangeActive && array.Length > 0)
			{
				Interactive closest = (Interactive) array.Min((area) => (int)area.GlobalPosition.DistanceSquaredTo(GetPlayer().GlobalPosition));

				var anim = _animSelector.GetDefaultAnimation(data);
				data.FacingRight = anim.FacingRight;
				data.CurrentAction = new ActionLookingAt(anim.Animation, closest.GetLookAtText());

				if (InputManager.IsActionJustPressed(InputButton.Interact))
				{
					InteractWith(closest);
				}
			}

			if (IsInControl)
			{
				data.CurrentAction = PlayerAnimation.Idle;
			}

		}

		private async Task InteractWith(Interactive what)
		{
			SetBehaviourChangesDisabled(true);
			SetBehavioursEnabled(DISABLED_DURING_INTERACTION, false);

			await what.Interact();

			SetBehavioursEnabled(DISABLED_DURING_INTERACTION, true);
			SetBehaviourChangesDisabled(false);
		}
	}
}