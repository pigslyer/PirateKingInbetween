using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Combos
{
	[ComboAttr(
		new AttrInput[]
		{
			AttrInput.SwitchDirection,
			AttrInput.Forward,
		}
	)]
	[ComboAttr(
		new AttrInput[]
		{
			AttrInput.Forward,
			AttrInput.Forward,
		}
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

		protected override async Task BeginCombo(IComboExecutor executor, ICombatFrameData startingData)
		{
			Vector2 d = DODGE_DIRECTION.FaceForward(startingData) * DODGE_DISTANCE;
			Vector2 initialPos = executor.GlobalPosition;
			Vector2 cameraDownD = CAMERA_DOWN_DIFF + d * DODGE_CAMERA_GOING_DOWN_PERCENT;
			Vector2 initialCamera = executor.CameraPosition;
			Vector2 afterDownCamera = initialCamera + cameraDownD;
			Vector2 cameraUpD = -CAMERA_DOWN_DIFF + d * (1f - DODGE_CAMERA_GOING_DOWN_PERCENT);

			startingData.Velocity = Vector2.Zero;

			DoFor(
				DODGE_CAMERA_DOWN_TIME,
				startingData : startingData,
				what : (float elapsed, float delta, float total, ICombatFrameData data) =>
				{
					executor.CameraPosition = GetCubicInterp(initialCamera, cameraDownD, elapsed, total);
				}
			)
			.ContinueWith(t => DoFor(
				DODGE_CAMERA_UP_TIME,
				startingData : null,
				what : (float elapsed, float delta, float total, ICombatFrameData data) =>
				{
					executor.CameraPosition = GetCubicInterp(afterDownCamera, cameraUpD, elapsed, total);
				}
			));

			await DoFor(
				DODGE_TIME,
				startingData : startingData,
				what : (float elapsed, float delta, float total, ICombatFrameData data) =>
				{
					executor.GlobalPosition = GetCubicInterp(initialPos, d, elapsed, total);
				}
			);
		}
	}
}