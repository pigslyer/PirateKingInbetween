using System.Diagnostics;
using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player
{
    public class PlayerController : KinematicBody2DOverride
	{
		private Vector2 _velocity = Vector2.Zero;

#region Paths
        [Export] private NodePath _playerModelPath = null;

		[Export] private NodePath _statesParentPath = null;
		[Export] private NodePath _debugLabelPath = null;
#endregion

		// Player's model.
        private PlayerModel _playerModel;
		// A random label for putting shit on.
		private Label _debugLabel;

		private ReadOnlyCollection<PlayerBehaviour> _behaviours;

		public override void _Ready()
		{
			base._Ready();

            _playerModel = GetNode<PlayerModel>(_playerModelPath);
			_debugLabel = GetNode<Label>(_debugLabelPath);
			
			var behaviours = new List<PlayerBehaviour>();

			foreach (var child in GetNode(_statesParentPath).GetChildren())
			{
				if (child is PlayerBehaviour behaviour)
				{
					behaviour.Initialize(this);
					behaviours.Add(behaviour);
				}
				else
				{
					PushWarning("Why the hell does States have a child that isn't a state?");
				}
			}

			_behaviours = new ReadOnlyCollection<PlayerBehaviour>(behaviours);
		}


#region Active behaviours
		// starting behaviours are all but noclip. these are the behaviours that have their code run, they don't necesarrily have to do anything (such as melee)
		public PlayerBehaviour.Behaviours ActiveBehaviours = PlayerBehaviour.Behaviours.Default;
		public void SetActiveBehaviours(PlayerBehaviour.Behaviours behaviours) => ActiveBehaviours = behaviours;

		public bool IsBehaviourActive(PlayerBehaviour.Behaviours behaviour) => (ActiveBehaviours & behaviour) != 0;


#endregion

		private bool _lastRight = true;

		public override void _PhysicsProcess(float delta)
		{
			var data = new PlayerCurrentFrameData(delta)
			{
				Velocity = _velocity,
				VelocityMult = 1f,
				FacingRight = _lastRight,
			};

			RunBehaviours(data);
			ApplyFrameData(data);

			DebugOutOfBounds();

			_debugLabel.Text = $"Animation: {Enum.GetName(typeof(PlayerAnimation), data.NextAnimation)}\nFacing right: {data.FacingRight}";
		}

		private void RunBehaviours(PlayerCurrentFrameData data)
		{
			for (int i = 0; i < _behaviours.Count; i++)
			{
				if (IsBehaviourActive((PlayerBehaviour.Behaviours)(1 << i)))
				{
					_behaviours[i].Run(data);
				}
			}

		}

		private void ApplyFrameData(PlayerCurrentFrameData data)
		{
			if (data.VelocityMult > 0f)
			{
				_velocity = MoveAndSlide(data.Velocity * data.VelocityMult, Vector2.Up) / data.VelocityMult;
			}

			_playerModel.SetAnimation(data.NextAnimation, data.FacingRight);

			_lastRight = data.FacingRight;

			if (data.AttackData != null)
			{
				if (data.AttackData is SlashData slash)
				{
					_playerModel.PlaySlash(slash);
				}
				else if (data.AttackData is ProjectileData bullet)
				{
					_playerModel.Shoot(bullet);
				}
			}
		}



		private void DebugOutOfBounds()
		{
			if (GlobalPosition.y > 300f)
			{
				GetTree().ReloadCurrentScene();
			}
		}

		private PlayerBehaviour.Behaviours _previous = PlayerBehaviour.Behaviours.Default;

		public override void _Input(InputEvent @event)
		{
			if (OS.IsDebugBuild() && @event.IsActionPressed("temp_noclip"))
			{
				if (IsBehaviourActive(PlayerBehaviour.Behaviours.Noclip))
				{
					ActiveBehaviours = _previous;
				}
				else
				{
					_previous = ActiveBehaviours;
					ActiveBehaviours = PlayerBehaviour.Behaviours.Noclip;
				}
			}
		}

	}

}
