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
		[Export] PackedScene _inWorldDescription = ReflectionHelper.LoadScene<FancyInWorldDisplay>();

#region Paths
		[Export] private NodePath __flippablePath = null;
		[Export] private NodePath __shootFromPath = null;
		[Export] private NodePath __damageDealerSlashPath = null;
		[Export] private NodePath __damageTakerBodyPath = null;
		[Export] private NodePath __damageDealerTempPreviewPath = null;
		[Export] private NodePath __interactionDisplayPath = null;
		[Export] private NodePath __playerSpritePath = null;
		[Export] private NodePath __playerSlashSpritesPath = null;
		[Export] private NodePath __playerPath = null;

		#endregion

		#region AnimationTimings

		[Export] private float _timeUntilSmokingStarts = 10f;

		#endregion

		private Node2D _flippable;
		private Position2D _shootFrom;
		private DamageDealer _damageDealerSlash;
		private CollisionShape2D _damageTakerBody;
		private ITextDisplay _currentLookAt;
		private Position2D _interactionDisplayPosition;
		private AnimatedSprite _playerSprite;
		private AnimatedSprite _playerSlashSprite;
		private PlayerController _player;
		

		public override void _Ready()
		{
			_flippable = GetNode<Node2D>(__flippablePath);
			_shootFrom = GetNode<Position2D>(__shootFromPath);
			_damageDealerSlash = GetNode<DamageDealer>(__damageDealerSlashPath);
			_damageTakerBody = GetNode<CollisionShape2D>(__damageTakerBodyPath);
			_interactionDisplayPosition = GetNode<Position2D>(__interactionDisplayPath);
			_playerSprite = GetNode<AnimatedSprite>(__playerSpritePath);
			_playerSlashSprite = GetNode<AnimatedSprite>(__playerSlashSpritesPath);
			_player = GetNode<PlayerController>(__playerPath);

			_playerSlashSprite.Hide();
		}

		public void UpdateModel(PlayerCurrentFrameData data)
		{
			PlayerAction action = data.CurrentAction;

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

			PlayAnimation(data);
		}


		private void PlayAnimation(PlayerCurrentFrameData data)
		{

			string DefaultAnim(Animations animation, bool facingRight) => ApplyFacing(animation.GetString(), facingRight);
			string ApplyFacing(string animation, bool facingRight) => $"{animation}{(facingRight ? "R" : "L")}";
			string ApplyWooden(string animation, bool isWooden) => isWooden ? $"Wood{animation}" : animation;
			string ApplySlash(string animation) => $"{animation}Slash";

			bool facingAnimation;
			string anim;

			float timeInAnim = ProcessTimeInAnim(data);

			IntInterval animLength = (0, -1);

			switch (data.Animation)
			{
				// this exists because the first and second slash of the combo come from the same actual animation
				case Animations.BasicCombo1:
				case Animations.BasicCombo2:
				
				const string sharedAnimName = "BasicCombo12";
				const int endOf1 = 5;

				anim = ApplyFacing(sharedAnimName, data.FacingRight);
				facingAnimation = true;
				
				animLength = data.Animation == Animations.BasicCombo1 ? (0, endOf1) : (endOf1, -1);

				break;
				case Animations.BasicCombo3:
				anim = data.Animation.GetString();
				facingAnimation = false;
				break;

				default:
				anim = DefaultAnim(data.Animation, data.FacingRight);
				facingAnimation = true;
				break;
			}

			anim = ApplyWooden(anim, Autoloads.Global.PlayerHasWoodenLeg);

			_flippable.Scale = new Vector2(data.FacingRight.Sign(), 1f);
			_playerSprite.FlipH = !facingAnimation && !data.FacingRight;

			if (_playerSprite.Frames.HasAnimation(anim))
			{
				if (data.CurrentAction.PercentageDone is float percDone)
				{
					if (animLength.End == -1)
					{
						animLength.End = _playerSprite.Frames.GetFrameCount(anim);
					}

					_playerSprite.Playing = false;
					_playerSprite.Animation = anim;
					_playerSprite.Frame = animLength.GetAtPercent(percDone);
				}
				else
				{
					_playerSprite.Play(anim);
				}
			}
			else
			{
				_playerSprite.Play(DefaultAnim(Animations.Idle, data.FacingRight));
			}

			string slash = ApplySlash(_playerSprite.Animation);
			if (_playerSlashSprite.Frames.HasAnimation(slash))
			{
				_playerSlashSprite.Show();
				_playerSlashSprite.Animation = slash;
				_playerSlashSprite.Frame = _playerSprite.Frame;
			}
			else
			{
				_playerSlashSprite.Hide();
			}
		}

		private float _timeInAnim = 0f;
		private Animations _prevAnim = Animations.Idle;

		private float ProcessTimeInAnim(PlayerCurrentFrameData data)
		{
			_timeInAnim += data.Delta;

			if (data.Animation != _prevAnim)
			{
				_timeInAnim = 0f;
				_prevAnim = data.Animation;
			}

			return _timeInAnim;
		}

		private async Task ShowTempDamageDealer(float duration)
		{
			Sprite preview = GetNode<Sprite>(__damageDealerTempPreviewPath);

			preview.Show();

			await this.WaitFor(duration);

			preview.Hide();
		}

		public void DamageDealerEnable(DamageDealerTargettingArea area, DamageData damageData)
		{
			var dealer = GetComboDamageDealer(area);
			dealer.Enable(damageData);
			GetNode<Sprite>(__damageDealerTempPreviewPath).Show();
		}
		public void DamageDealerDisable(DamageDealerTargettingArea area)
		{
			GetComboDamageDealer(area).Disable();
			GetNode<Sprite>(__damageDealerTempPreviewPath).Hide();
		}

		// spreading damagetakers across multiple areas'd be frustrating and make no sense.
		public void DamageTakerEnable(DamageTakerTargetArea area) => GetComboDamageTaker(area).Disabled = false;

		public void DamageTakerDisable(DamageTakerTargetArea area) => GetComboDamageTaker(area).Disabled = true;



		private DamageDealer GetComboDamageDealer(DamageDealerTargettingArea area)
		{
			switch (area)
			{
				default:
				case DamageDealerTargettingArea.Front:
				return _damageDealerSlash;
			}
		}

		private CollisionShape2D GetComboDamageTaker(DamageTakerTargetArea area)
		{
			switch (area)
			{
				default:
				case DamageTakerTargetArea.Body:
				return _damageTakerBody;
			}
		}
	}
}

