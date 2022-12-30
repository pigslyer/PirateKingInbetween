using Godot;
using System;

namespace PirateInBetween.Player
{
    public class PlayerController : KinematicBody2D
	{
		private Vector2 _velocity = Vector2.Zero;

#region Paths
        [Export] private NodePath _playerModelPath;

		[Export] private NodePath _statesParentPath;
		[Export] private NodePath _playerMovementPath;
#endregion

		// Player's model.
        private PlayerModel _playerModel;
		private PlayerMovement _playerMovement;

		public override void _Ready()
		{
            _playerModel = GetNode<PlayerModel>(_playerModelPath);
			_playerMovement = GetNode<PlayerMovement>(_playerMovementPath);

			foreach (var child in GetNode(_statesParentPath).GetChildren())
			{
				if (child is PlayerState state)
				{
					state.Initialize(this);
				}
				else
				{
					GD.PushWarning("Why the hell does States have a child that isn't a state?");
				}
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			_velocity = MoveAndSlide(_playerMovement.RecalculateVelocity(_velocity, delta), Vector2.Up);
		}




		public static Vector2 GetInputVector() => Input.GetVector("mv_left", "mv_right", "mv_up", "mv_down");
	}

}
