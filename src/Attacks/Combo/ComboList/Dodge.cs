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
		holdForward : UsageReq.Optional,
		onFloor : UsageReq.Optional
	)]
	[ComboAttr(
		inputs : new AttrInput[]
		{
			AttrInput.Forward,
			AttrInput.Forward,
		},
		holdForward : UsageReq.Optional,
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

		protected override void BeginCombo()
		{
			CurrentData.Velocity = Vector2.Zero;

			AddTask().DisableDamageFor(DODGE_TIME, ComboExecutorDamageTaker.Body);

			AddTask()
			.CubicInterpFor(
				from : Vector2.Zero,
				to : CAMERA_DOWN_DIFF,
				setter : val => CurrentExecutor.CameraPosition = val,
				time : DODGE_CAMERA_DOWN_TIME
			)
			.CubicInterpFor(
				from : CAMERA_DOWN_DIFF,
				to : Vector2.Zero,
				setter : val => CurrentExecutor.CameraPosition = val,
				time : DODGE_CAMERA_UP_TIME
			);

			AddTask().CubicInterpFor(
				from : CurrentExecutor.GlobalPosition,
				to : CurrentExecutor.GlobalPosition + DODGE_DIRECTION.FaceForward(CurrentData) * DODGE_DISTANCE,
				setter : val => CurrentExecutor.GlobalPosition = val,
				time : DODGE_TIME
			);
		}
	}
}