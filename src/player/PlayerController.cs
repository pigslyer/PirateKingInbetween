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

			GlobalGetter.Player = this;
		}


		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			_lastAnimation = CharacterAnimation.None;
			_behaviourManager.RunBehaviours();

			MoveAndSlide();
		}


		public float Delta => (float)GetPhysicsProcessDeltaTime();	
		public new bool IsOnFloor => IsOnFloor();
		public CharacterAnimation CurrentAnimation
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
					PlayerBehaviour.Generate<PlayerBehaviourCamera      >(this),
					PlayerBehaviour.Generate<PlayerBehaviourMovement	>(this),
					PlayerBehaviour.Generate<PlayerBehaviourShoot		>(this),
					PlayerBehaviour.Generate<PlayerBehaviourModel		>(this),

				}
			);
		}
	}
}