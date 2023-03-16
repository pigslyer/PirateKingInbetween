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

		private const float TOTAL_SHOOT_TIME = 0.5f;
		private const float TIME_TO_FIRE = 0.3f;

		protected override void BeginCombo()
		{
			AddTask().DoFor(TOTAL_SHOOT_TIME, (elapsed,delta,total) => CurrentData.Anim = new CombatAnimation(
				Animations.BasicShoot1, elapsed / total
			));

			AddTask().WaitFor(0.3f).Do(() =>
			{
				var bullet = _bulletScene.Instance<StraightBullet>();
				bullet.Initialize(PhysicsLayers.World, new DamageAmount(amount: 10, stun: 0.3f), 0f.FaceForward(CurrentData) * 200f);
				CurrentExecutor.Shoot(bullet);
			});
		}
	}
}