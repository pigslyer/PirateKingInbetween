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
		inputs: new AttrInput[]{
			AttrInput.Ranged,
		},
		holdForward: UsageReq.Never,
		onFloor: UsageReq.Required
	)]
	public class BasicShoot : Combo
	{
		private static readonly PackedScene _bulletScene = ReflectionHelper.LoadScene<StraightBullet>();

		protected override void BeginCombo()
		{
			//AddTask().DoFor(0.5f, (a, b, c) => CurrentData.ResetHorizontal());

			AddTask().WaitFor(0.3f).Do(() =>
			{
				var bullet = _bulletScene.Instance<StraightBullet>();
				bullet.Initialize(PhysicsLayers.World, 0, 0f.FaceForward(CurrentData) * 200f);
				CurrentExecutor.Shoot(bullet);
			});
		}
	}
}