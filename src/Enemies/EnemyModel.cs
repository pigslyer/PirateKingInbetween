using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;

namespace PirateInBetween.Game.Enemies
{
	public class EnemyModel : Node2D
	{
		private static readonly PackedScene BUBBLE_SCENE = ReflectionHelper.LoadScene<FancyInWorldDisplay>();

		[Export] private Texture _stunBubble = null;
		[Export] private Texture _attackBubble = null;

		#region Paths

		[Export] private NodePath __BubbleParentPath = null;
		[Export] private NodePath __shootFromPath = null;

		#endregion

		private Node2D _bubbleParent;
		private Node2D _shootFrom;
		private EnemyController _controller;

		public override void _Ready()
		{
			base._Ready();

			_bubbleParent = GetNode<Node2D>(__BubbleParentPath);
			_shootFrom = GetNode<Node2D>(__shootFromPath);
		}

		public void Initialize(EnemyController controller)
		{
			_controller = controller;
			GenerateDamageDealersArray(controller); GenerateDamageTakersArray(controller);
		}

		#region Damage


		private EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea> _damageDealers;

		private EnumToCollectionMap<DamageTaker, DamageTakerTargetArea> _damageTakers;

		private void GenerateDamageDealersArray(EnemyController root)
		{
			_damageDealers = new EnumToCollectionMap<DamageDealer, DamageDealerTargettingArea>(root.GetAllProgenyNodesOfType<DamageDealer>(), d => d.DealerType);
		}

		private void GenerateDamageTakersArray(EnemyController root)
		{
			_damageTakers = new EnumToCollectionMap<DamageTaker, DamageTakerTargetArea>(root.GetAllProgenyNodesOfType<DamageTaker>(), t => t.TakerType);
		}


		public void DealDamage(DamageDealerTargettingArea damageDealer, DamageData data) => _damageDealers.DoFor(d => d.Enable(data), damageDealer);

		public void StopDealingDamage(DamageDealerTargettingArea damageDealer) => _damageDealers.DoFor(d => d.Disable(), damageDealer);
		public void Shoot(Projectile p) => p.Shoot(_shootFrom.GlobalPosition, _controller.GetMovingParentDetector().CurrentMovingParent);

		public void TakeDamage(DamageTakerTargetArea damageTaker) => _damageTakers.DoFor(t => t.Enable(), damageTaker);
		public void StopTakingDamage(DamageTakerTargetArea damageTaker) => _damageTakers.DoFor(t => t.Disable(), damageTaker);

		#endregion
	
		public virtual void PlayAnimation(Animations animation)
		{ }

		private const float ADDITIONAL_STUN_ANIM_DELAY = 0.5f;
		private float _stunDuration;

		public async void PlayStun(float duration)
		{
			// if we're stunned we cancel attack
			if (duration > 0f)
			{
				PlayAttack(false);
			}

			if (_stunDuration > 0f)
			{
				_stunDuration = (duration == 0f ? 0f : duration + ADDITIONAL_STUN_ANIM_DELAY);
				return;
			}

			var bubble = BUBBLE_SCENE.Instance<FancyInWorldDisplay>();

			_stunDuration = duration + ADDITIONAL_STUN_ANIM_DELAY;

			bubble.Appear(_bubbleParent, _bubbleParent.GlobalPosition, _stunBubble);
			bubble.Show();

			while (_stunDuration > 0f)
			{
				// need some kind of visibility interpolation
				//bubble.Visible = !bubble.Visible;

				await this.AwaitIdle();

				_stunDuration -= GetProcessDeltaTime();
			}
			
			bubble.Show();
			bubble.Disappear();
		}

		private bool _attackState = false;
		public async void PlayAttack(bool state)
		{
			if (_attackState == state)
			{
				return;
			}

			_attackState = state;

			if (!_attackState)
			{
				return;
			}

			var bubble = BUBBLE_SCENE.Instance<FancyInWorldDisplay>();

			bubble.Appear(_bubbleParent, _bubbleParent.GlobalPosition, _attackBubble);
			bubble.Show();

			while (state == _attackState)
			{
				await this.AwaitIdle();
			}

			bubble.Show();
			bubble.Disappear();
		}
	}
}