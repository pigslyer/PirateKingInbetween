using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Pigslyer.PirateKingInbetween.Util.Behaviour;
using Pigslyer.PirateKingInbetween.Player.Behaviours;

namespace Pigslyer.PirateKingInbetween.Player
{
	public partial class PlayerController : CharacterBody2DOverride, IFrameData
	{
		[Export] public PlayerBehaviourProperties BehaviourProperties = null!;

		[Signal] public delegate void OnDamageTakenEventHandler();

		private BehaviourManager _behaviourManager = null!;
		private CharacterAnimation _lastAnimation = CharacterAnimation.None;

		public override void _Ready()
		{
			_behaviourManager = GenerateDefaultBehaviours();
		}

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			_behaviourManager.RunBehaviours();

			MoveAndSlide();
		}

		float IFrameData.Delta => (float)GetPhysicsProcessDeltaTime();	
		bool IFrameData.IsOnFloor => IsOnFloor();
		CharacterAnimation IFrameData.CurrentAnimation
		{
			get => _lastAnimation;
			set
			{
				if (_lastAnimation == CharacterAnimation.None)
				{
					_lastAnimation = value;
				}
			}
		}

		private BehaviourManager GenerateDefaultBehaviours()
		{
			return BehaviourManager.Generate(
				new IBehaviour[]
				{
					new PlayerBehaviourHorizontalMovement(this),
					new PlayerBehaviourFalling(this),
					new PlayerBehaviourJumping(this),
					new PlayerBehaviourModel(this),
				}
			);
		}

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);

			if (@event.IsAction("ui_accept"))
			{
				Position = new(Position.X, 0);
			}
		}
	}
}