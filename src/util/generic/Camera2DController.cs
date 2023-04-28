using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Generic
{
	public partial class Camera2DController : Camera2D
	{
		private enum CameraMode
		{
			Static,
			FollowNode,
		};

		private CameraMode _currentMode = CameraMode.Static;
		private Node2D _followingNode;

		private float _viewBobStrength = 0.0f;
		private float _viewBobSpeed = 0.0f;

		public Camera2DController()
		{
			_followingNode = this;
		}

		public void SetFollowNode(Node2D node2d)
		{
			_currentMode = CameraMode.FollowNode; _followingNode = node2d;
		}

		public void SetBob(float strength, float cyclesPerSeconds)
		{
			_viewBobStrength = strength; _viewBobSpeed = cyclesPerSeconds * Mathf.Tau;
		}

		private float _time = 0;

		public override void _Process(double delta)
		{
			base._Process(delta);

			GlobalPosition = _currentMode switch
			{
				CameraMode.FollowNode => _followingNode.GlobalPosition,
				_ => GlobalPosition,
			};

			Position += new Vector2(0.0f, _viewBobStrength * Mathf.Sin(_viewBobSpeed * _time));

			_time += (float)delta;
		}
	}
}