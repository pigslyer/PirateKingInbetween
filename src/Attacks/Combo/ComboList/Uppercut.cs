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
			AttrInput.Melee, 
			AttrInput.Up, 
		},
		holdForward : UsageReq.Never,
		onFloor : UsageReq.Required
	)]
	public class Uppercut : Combo
	{
		private const float FLY_DISTANCE = 80f;
		private const float FLY_TIME = 0.4f;
		private const float FLY_DIRECTION = 80f;

		private const float FLOAT_TIME = 0.8f;
		private static readonly Vector2 UPWARD_DIR = new Vector2(0f, 5f);

		protected async override Task BeginCombo(IComboExecutor callingNode)
		{
			Vector2 initialPos = callingNode.GlobalPosition;
			Vector2 diff = FLY_DIRECTION.FaceForward(CurrentData) * FLY_DISTANCE;

			CurrentData.Velocity = Vector2.Zero;

			await DoFor(
				time: FLY_TIME,
				what: (float elapsed, float delta, float total) =>
				{
					callingNode.GlobalPosition = (Vector2) GetCubicInterp(initialPos, diff, elapsed, total);
				}
			);

			initialPos = callingNode.GlobalPosition;

			await DoFor(
				time: FLOAT_TIME,
				what: (float elapsed, float delta, float total) =>
				{
					callingNode.GlobalPosition = GetCubicInterp(initialPos, UPWARD_DIR, elapsed, total);
				}
			);
		}
	}
}