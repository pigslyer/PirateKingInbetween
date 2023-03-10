using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game
{
    public class CameraController : Camera2D
    {
        
        #region Paths

        [Export] private NodePath _cameraFollowingPath = null;

        #endregion

        /// <summary>
        /// The thing we're following at the moment.
        /// </summary>
        private Node2D _cameraFollowing;
        private bool _isFollowing = true;

        public override void _Ready()
        {
            base._Ready();

            _cameraFollowing = GetNode<Node2D>(_cameraFollowingPath);
            GlobalPosition = _cameraFollowing.GlobalPosition;
            ResetSmoothing();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            GlobalPosition = _cameraFollowing.GlobalPosition;
        }

        public void SetFollowing(bool state)
        {
            if (_isFollowing != state)
            {
				//SmoothingEnabled = state;
				SetProcess(state);
                _isFollowing = state;
			}
        }

        public Vector2 RelativePosition
        {
            get => GlobalPosition - _cameraFollowing.GlobalPosition;
            set => GlobalPosition = _cameraFollowing.GlobalPosition + value;
        }
    }
}