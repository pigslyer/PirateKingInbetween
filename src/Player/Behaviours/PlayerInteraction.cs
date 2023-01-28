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

		public override void _Ready()
		{
			base._Ready();

            _interactionDetectionArea = GetNode<Godot.Area2D>(__interactionDetectionAreaPath);
			_animSelector = GetSiblingBehaviour<PlayerAnimationSelector>(BehavioursPos.AnimationSelector);
		}

		public override void Run(PlayerCurrentFrameData data)
		{
			var array = _interactionDetectionArea.GetOverlappingAreas();

			if (!data.IsBusy && array.Count > 0)
			{
				Interactive inter = (Interactive) array[0];

				if (InputManager.IsActionJustPressed(InputButton.Interact))
				{
					inter.Interact();
				}
				else
				{
					var anim = _animSelector.GetDefaultAnimation(data);
					data.FacingRight = anim.FacingRight;
					data.CurrentAction = new ActionLookingAt(anim.Animation, inter.GetLookAtText());
				}
			}
		}
	}
}