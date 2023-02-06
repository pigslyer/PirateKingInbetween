using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos.List
{
	[ComboAttr(
		inputs : new AttrInput[]
		{
			AttrInput.SwitchDirection,
			AttrInput.Forward,
		}, 
		holdForward : UsageReq.Required,
		onFloor : UsageReq.Optional
	)]
	[ComboAttr(
		inputs : new AttrInput[]
		{
			AttrInput.Forward,
			AttrInput.Forward,
		},
		holdForward : UsageReq.Required,
		onFloor : UsageReq.Optional
	)]
	public class Dodge : Combo
	{
		private const float DODGE_TIME = 0.4f;
		private const float DODGE_DISTANCE = 100f;
		private const float DODGE_DIRECTION = 0f;

		private const float DODGE_CAMERA_GOING_DOWN_PERCENT = 0.3f;
		private const float DODGE_CAMERA_DOWN_TIME = DODGE_TIME * DODGE_CAMERA_GOING_DOWN_PERCENT;
		private const float DODGE_CAMERA_UP_TIME = DODGE_TIME - DODGE_CAMERA_DOWN_TIME;
		private static readonly Vector2 CAMERA_DOWN_DIFF = new Vector2(0, 12);

		protected override async Task BeginCombo(IComboExecutor executor)
		{
			CurrentData.Velocity = Vector2.Zero;

			CubicInterpFor(
				from : Vector2.Zero,
				to : CAMERA_DOWN_DIFF,
				setter : val => executor.CameraPosition = val,
				time : DODGE_CAMERA_DOWN_TIME
			)
			.ContinueWith(t => CubicInterpFor(
				from : CAMERA_DOWN_DIFF,
				to : Vector2.Zero,
				setter : val => executor.CameraPosition = val,
				time : DODGE_CAMERA_UP_TIME
			));

			await CubicInterpFor(
				from : executor.GlobalPosition,
				to : executor.GlobalPosition + DODGE_DIRECTION.FaceForward(CurrentData) * DODGE_DISTANCE,
				setter : val => executor.GlobalPosition = val,
				time : DODGE_TIME
			);
		}
	}
}