using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Props
{
	public partial class PressurePlate : Node2D
	{
		[Signal] public delegate void OnPlatePressedEventHandler(bool state);

		[Export] private Area2D _targetArea = null!;
		[Export] private Sprite2D _sprite = null!;

		private bool _isPressed = false;

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			DebugDraw.DrawTextMouse($"overlapping: {string.Join(", ", _targetArea.GetOverlappingBodies().Select(n => n.Name))}");
			if (_isPressed == (_targetArea.GetOverlappingBodies().Count == 0))
			{
				SetPressed(!_isPressed);
			}
		}

		private const float PRESSURED_PLATE_DROP = 3;
		private const float PRESSURED_PLATE_DROP_TIME = 0.4f;
		
		private void SetPressed(bool state)
		{
			if (_isPressed == state)
			{
				return;
			}

			_isPressed = state;
			
			var tween = CreateTween();
			tween.TweenProperty(_sprite, "position", (_isPressed ? PRESSURED_PLATE_DROP : 0).Vec0Y(), PRESSURED_PLATE_DROP_TIME);

			EmitSignal(SignalName.OnPlatePressed, _isPressed);
		}
	}
}