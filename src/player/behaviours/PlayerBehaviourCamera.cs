using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player.Behaviours
{
	public class PlayerBehaviourCamera : PlayerBehaviour
	{
		private Node2D _cameraFollowingNode = null!;
		private PlayerBehaviourModel _model = null!;
		
		public override void InitializeBehaviour()
		{
			_cameraFollowingNode = new();
			Controller.AddChild(_cameraFollowingNode);

			_cameraController.SetFollowNode(_cameraFollowingNode);

			_model = GetSpecificBehaviour<PlayerBehaviourModel>();
		}

		public override void ActiveBehaviour()
		{ }

		private Camera2DController _cameraController => BehaviourProperties.CameraController;
		private float _cameraOffset => BehaviourProperties.CameraOffset;
		private float _cameraBobStrength => BehaviourProperties.CameraBobStrength;
		private float _cameraBobSpeed => BehaviourProperties.CameraBobSpeed;

		public override void PassiveBehaviour()
		{
			float sign = Mathf.Sign(Velocity.X);

			if (Velocity.X != 0 || !Controller.IsOnFloor)
			{
				_cameraFollowingNode.Position = (new Vector2(_model.IsFacingRight ? _cameraOffset : -_cameraOffset, 0) + Velocity).Normalized() * _cameraOffset;
			}
			else
			{
				_cameraFollowingNode.Position = new Vector2(_model.IsFacingRight ? _cameraOffset : -_cameraOffset, 0);
			}

			DebugDraw.DrawArrow(Controller.GlobalPosition, _cameraFollowingNode.GlobalPosition, Colors.Green);

			_cameraController.SetBob(sign * _cameraBobStrength, sign * _cameraBobSpeed);
		}
	}
}