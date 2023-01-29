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
		[Export] PackedScene _inWorldDescription = null;

#region Paths
		[Export] private NodePath __flippablePath = null;
		[Export] private NodePath __shootFromPath = null;
		[Export] private NodePath __damageDealerSlashPath = null;
		[Export] private NodePath __damageDealerTempPreviewPath = null;
		[Export] private NodePath __interactionDisplayPath = null;
		[Export] private NodePath __playerPath = null;

		#endregion

		private Node2D _flippable;
		private Position2D _shootFrom;
		private DamageDealer _damageDealerSlash;
		private ITextDisplay _currentLookAt;
		private Position2D _interactionDisplayPosition;
		private PlayerController _player;

		public override void _Ready()
		{
			_flippable = GetNode<Node2D>(__flippablePath);
			_shootFrom = GetNode<Position2D>(__shootFromPath);
			_damageDealerSlash = GetNode<DamageDealer>(__damageDealerSlashPath);
			_interactionDisplayPosition = GetNode<Position2D>(__interactionDisplayPath);
			_player = GetNode<PlayerController>(__playerPath);
		}

		public void SetAnimation(PlayerAction action, bool facingRight)
		{
			_flippable.Scale = new Vector2(facingRight ? 1f : -1f, 1f);

			if (action is ActionMeleeAttack melee)
			{
				_damageDealerSlash.TempEnable(melee.DamageData, melee.SlashDuration);
				ShowTempDamageDealer(melee.SlashDuration);
			}
			else if (action is ActionRangedAttack ranged)
			{
				ranged.Bullet.Shoot(_shootFrom.GlobalPosition, _player.GetMovingParentDetector().CurrentMovingParent);
			}
			else if (action is ActionLookingAt lookat)
			{
				if (_currentLookAt == null || _currentLookAt.CurrentText() != lookat.Text)
				{
					_currentLookAt?.Disappear();
					_currentLookAt = _inWorldDescription.Instance<ITextDisplay>();
					_currentLookAt.Appear(_interactionDisplayPosition, _interactionDisplayPosition.GlobalPosition, lookat.Text);
				}
			}
			else
			{
				if (_currentLookAt != null)
				{
					_currentLookAt.Disappear();
					_currentLookAt = null;
				}
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

