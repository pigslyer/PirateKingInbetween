using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;
using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player
{
	public class PlayerModel : Node2D
	{
		[Export] PackedScene _inWorldDescription = ReflectionHelper.LoadScene<FancyInWorldText>();

#region Paths
		[Export] private NodePath __flippablePath = null;
		[Export] private NodePath __shootFromPath = null;
		[Export] private NodePath __damageDealerSlashPath = null;
		[Export] private NodePath __damageTakerBodyPath = null;
		[Export] private NodePath __damageDealerTempPreviewPath = null;
		[Export] private NodePath __interactionDisplayPath = null;
		[Export] private NodePath __playerSpritePath = null;
		[Export] private NodePath __playerPath = null;

		#endregion

		private Node2D _flippable;
		private Position2D _shootFrom;
		private DamageDealer _damageDealerSlash;
		private CollisionShape2D _damageTakerBody;
		private ITextDisplay _currentLookAt;
		private Position2D _interactionDisplayPosition;
		private AnimatedSprite _playerSprite;
		private PlayerController _player;

		public override void _Ready()
		{
			_flippable = GetNode<Node2D>(__flippablePath);
			_shootFrom = GetNode<Position2D>(__shootFromPath);
			_damageDealerSlash = GetNode<DamageDealer>(__damageDealerSlashPath);
			_damageTakerBody = GetNode<CollisionShape2D>(__damageTakerBodyPath);
			_interactionDisplayPosition = GetNode<Position2D>(__interactionDisplayPath);
			_playerSprite = GetNode<AnimatedSprite>(__playerSpritePath);
			_player = GetNode<PlayerController>(__playerPath);

			_playerSprite.Play("Idle");
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

		public void DamageDealerEnable(ComboExecutorDamageDealers area, DamageData damageData)
		{
			var dealer = GetComboDamageDealer(area);
			dealer.Enable(damageData);
			GetNode<Sprite>(__damageDealerTempPreviewPath).Show();
		}
		public void DamageDealerDisable(ComboExecutorDamageDealers area)
		{
			GetComboDamageDealer(area).Disable();
			GetNode<Sprite>(__damageDealerTempPreviewPath).Hide();
		}

		// spreading damagetakers across multiple areas'd be frustrating and make no sense.
		public void DamageTakerEnable(ComboExecutorDamageTaker area) => GetComboDamageTaker(area).Disabled = false;

		public void DamageTakerDisable(ComboExecutorDamageTaker area) => GetComboDamageTaker(area).Disabled = true;



		private DamageDealer GetComboDamageDealer(ComboExecutorDamageDealers area)
		{
			switch (area)
			{
				default:
				case ComboExecutorDamageDealers.Front:
				return _damageDealerSlash;
			}
		}

		private CollisionShape2D GetComboDamageTaker(ComboExecutorDamageTaker area)
		{
			switch (area)
			{
				default:
				case ComboExecutorDamageTaker.Body:
				return _damageTakerBody;
			}
		}
	}
}

