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
	public class PlayerRangedAttack : PlayerBehaviour
	{
		[Export] private PackedScene _bulletScene = null;
		[Export] private int _damage = 1; 
		[Export] private float _bulletSpeed = 200f;

		public override void Run(PlayerCurrentFrameData data)
		{
			if (InputManager.IsActionJustPressed(InputButton.RangedAttack))
			{
				var bullet = _bulletScene.Instance<StraightBullet>();

				bullet.SetData(
						damageData : new DamageData(_damage, () => bullet.GlobalPosition), 
						data : new StraightBullet.Data(new Vector2(_bulletSpeed * (data.FacingRight ? 1 : -1), 0f))
				);

				data.CurrentAction = new ActionRangedAttack(bullet);
			}
		}
	}
}