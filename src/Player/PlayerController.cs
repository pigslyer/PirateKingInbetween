using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Player
{
    public class PlayerController : KinematicBody2D
	{
		private Vector2 _velocity = Vector2.Zero;

#region Paths
        [Export] private NodePath _playerModelPath;

		[Export] private NodePath _statesParentPath;
		[Export] private NodePath _debugLabelPath;
#endregion

		// Player's model.
        private PlayerModel _playerModel;
		// A random label for putting shit on.
		private Label _debugLabel;

		private ReadOnlyCollection<PlayerBehaviour> _behaviours;

		public override void _Ready()
		{
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
					GD.PushWarning("Why the hell does States have a child that isn't a state?");
				}
			}

			_behaviours = new ReadOnlyCollection<PlayerBehaviour>(behaviours);
		}

		private bool _lastRight = true;

		public override void _PhysicsProcess(float delta)
		{
			var data = new PlayerCurrentFrameData(delta) {
				Velocity = _velocity, 
				VelocityMult = 1f,
				FacingRight = _lastRight,
			};

			// calling behaviours

			foreach (var behaviour in _behaviours)
			{
				behaviour.Run(data);
			}

			// applying behaviours

			_velocity = MoveAndSlide(data.Velocity * data.VelocityMult, Vector2.Up);
			_playerModel.SetAnimation(data.NextAnimation, data.FacingRight);

			_lastRight = data.FacingRight;

			_debugLabel.Text = $"Animation: {Enum.GetName(typeof(PlayerAnimation), data.NextAnimation)}\nFacing right: {data.FacingRight}";
		}



	}

}
