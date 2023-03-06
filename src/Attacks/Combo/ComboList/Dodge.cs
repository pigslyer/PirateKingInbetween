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
		inputs: new AttrInput[]
		{
			AttrInput.Forward | AttrInput.Dodge
		},
		holdForward: UsageReq.Optional,
		onFloor: UsageReq.Required
	)]
	public class Dodge : Combo
	{
		private const float DODGE_TIME = 1f;
		private const float DODGE_DISTANCE = 100f;
		private const float DODGE_DIRECTION = 0f;

		private const float DODGE_CAMERA_GOING_DOWN_PERCENT = 0.3f;
		private const float DODGE_CAMERA_DOWN_TIME = DODGE_TIME * DODGE_CAMERA_GOING_DOWN_PERCENT;
		private const float DODGE_CAMERA_UP_TIME = DODGE_TIME - DODGE_CAMERA_DOWN_TIME;
		private static readonly Vector2 CAMERA_DOWN_DIFF = new Vector2(0, 12);

		protected override void BeginCombo()
		{
			CurrentData.Velocity = Vector2.Zero;

			AddTask().DisableDamageFor(DODGE_TIME, DamageTakerTargetArea.Body);

			AddTask()
			.InterpFor(
				from : Vector2.Zero,
				to : CAMERA_DOWN_DIFF,
				setter : val => CurrentExecutor.CameraPosition = val,
				time : DODGE_CAMERA_DOWN_TIME
			)
			.InterpFor(
				from : CAMERA_DOWN_DIFF,
				to : Vector2.Zero,
				setter : val => CurrentExecutor.CameraPosition = val,
				time : DODGE_CAMERA_UP_TIME
			);

			// ensures a slower fall
			AddTask().DoFor(DODGE_TIME, (float elapsed, float delta, float total) => CurrentData.Velocity = new Vector2(CurrentData.Velocity.x, CurrentData.Velocity.y * 0.5f));

			AddTask().DoIf(
				DODGE_TIME, 
				() => !(CurrentData.IsGoingForward() && InputManager.IsActionPressed(InputButton.Dodge)), 
				() =>
				{
					CurrentExecutor.TakeDamage(DamageTakerTargetArea.Body); 
					Stop();
				}
			);

//			EnableHorizontalControl(DODGE_TIME, DODGE_DISTANCE / DODGE_TIME * 4f, DODGE_DISTANCE / DODGE_TIME);

			AddTask().InterpFor<float>(
				from : CurrentExecutor.GlobalPosition.x,
				delta : CurrentData.GetDirection() * DODGE_DISTANCE,
				setter : val => CurrentExecutor.GlobalPosition = new Vector2(val, CurrentExecutor.GlobalPosition.y),
				time : DODGE_TIME,
				trans: Tween.TransitionType.Sine
			);

		}
	}
}