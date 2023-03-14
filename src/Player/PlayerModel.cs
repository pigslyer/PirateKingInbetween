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
		[Export] private NodePath __damageDealerTempPreviewPath = null;
		[Export] private NodePath __interactionDisplayPath = null;
		[Export] private NodePath __playerSpritePath = null;
		[Export] private NodePath __playerSlashSpritesPath = null;
		
#endregion

		#region AnimationTimings

		[Export] private float _timeUntilSmokingStarts = 10f;

		#endregion

		private Node2D _flippable;
		private Position2D _shootFrom;

		private EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea> _damageDealers;
		private EnumToCollectionMap<IDamageTaker, DamageTakerTargetArea> _damageTakers;

		private ITextDisplay _currentLookAt;
		private Position2D _interactionDisplayPosition;
		private AnimatedSprite _playerSprite;
		private AnimatedSprite _playerSlashSprite;
		private PlayerController _player;
		

		public override void _Ready()
		{
			_flippable = GetNode<Node2D>(__flippablePath);
			_shootFrom = GetNode<Position2D>(__shootFromPath);
			_interactionDisplayPosition = GetNode<Position2D>(__interactionDisplayPath);
			_playerSprite = GetNode<AnimatedSprite>(__playerSpritePath);
			_playerSlashSprite = GetNode<AnimatedSprite>(__playerSlashSpritesPath);
			
			_playerSlashSprite.Hide();
		}

		public void Initialize(PlayerController player)
		{
			_player = player;
			
			_damageDealers = new EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea>(
				from: player.GetAllProgenyNodesOfType<DamageDealer>(),
				determineMapping: dealer => dealer.DealerType
			);			
			_damageTakers = new EnumToCollectionMap<IDamageTaker, DamageTakerTargetArea>(
				from: player.GetAllProgenyNodesOfType<IDamageTaker>(),
				determineMapping: taker => taker.TakerType
			);
		}

		public void UpdateModel(PlayerCurrentFrameData data)
		{
			PlayerAction action = data.CurrentAction;
			
			if (action is ActionRangedAttack ranged)
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

		private const float ANIM_HANG_TIME = 0.5f;
		private bool _hangOnAnim = false, _shouldHangOnAnimLast;

		private void PlayAnimation(PlayerCurrentFrameData data)
		{

			string DefaultAnim(Animations animation, bool facingRight) => ApplyFacing(animation.GetString(), facingRight);
			string ApplyFacing(string animation, bool facingRight) => $"{animation}{(facingRight ? "R" : "L")}";
			string ApplyWooden(string animation, bool isWooden) => isWooden ? $"Wood{animation}" : animation;
			string ApplySlash(string animation) => $"{animation}Slash";

			bool facingAnimation;
			bool shouldHangOnAnim = false;
			string anim;

			float timeInAnim = ProcessTimeInAnim(data);

			if (_hangOnAnim && data.Animation == Animations.Idle && timeInAnim < ANIM_HANG_TIME)
			{
				return;
			}

			_hangOnAnim = false;

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
				shouldHangOnAnim = true;
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

			if (_shouldHangOnAnimLast && anim != _playerSprite.Animation)
			{
				_shouldHangOnAnimLast = false;
				_hangOnAnim = true;
				return;
			}

			_shouldHangOnAnimLast = shouldHangOnAnim;

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
			_damageDealers.DoFor(
				what: dealer => dealer.Enable(damageData),
				type: area
			);
			GetNode<Sprite>(__damageDealerTempPreviewPath).Show();
		}
		public void DamageDealerDisable(DamageDealerTargettingArea area)
		{
			_damageDealers.DoFor(
				what: dealer => dealer.Disable(),
				type: area
			);
			GetNode<Sprite>(__damageDealerTempPreviewPath).Hide();
		}

		public void DamageTakerEnable(DamageTakerTargetArea area) => _damageTakers.DoFor(what: taker => taker.Enable(), type: area);

		public void DamageTakerDisable(DamageTakerTargetArea area) => _damageTakers.DoFor(what: taker => taker.Disable(), type: area);
	}
}

