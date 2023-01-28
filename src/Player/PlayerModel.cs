using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player
{
	public class PlayerModel : Node2D
	{

#region Paths

		[Export] private NodePath __shootFromPath = null;
		[Export] private NodePath __damageDealerSlashPath = null;

		[Export] private NodePath __damageDealerTempPreviewPath = null;
		[Export] private NodePath __playerPath = null;
#endregion

		private Position2D _shootFrom;
		private DamageDealer _damageDealerSlash;
		private PlayerController _player;

		public override void _Ready()
		{
			_shootFrom = GetNode<Position2D>(__shootFromPath);
			_damageDealerSlash = GetNode<DamageDealer>(__damageDealerSlashPath);
			_player = GetNode<PlayerController>(__playerPath);
		}

		public void SetAnimation(PlayerAction action, bool facingRight)
		{
			Scale = new Vector2(facingRight ? 1f : -1f, 1f);

			if (action is ActionMeleeAttack melee)
			{
				_damageDealerSlash.TempEnable(melee.DamageData, melee.SlashDuration);
				ShowTempDamageDealer(melee.SlashDuration);
			}
			else if (action is ActionRangedAttack ranged)
			{
				ranged.Bullet.Shoot(_shootFrom.GlobalPosition, _player.GetMovingParentDetector().CurrentMovingParent);
			}
		}

		private async Task ShowTempDamageDealer(float duration)
		{
			Sprite preview = GetNode<Sprite>(__damageDealerTempPreviewPath);

			preview.Show();

			await this.WaitFor(duration);

			preview.Hide();
		}
	}
}

