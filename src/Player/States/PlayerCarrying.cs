using System.Collections.Specialized;
using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player
{
    public class PlayerCarrying : PlayerBehaviour
    {
        [Export] private float _pushingVelMult = 0.3f;

        [Export] private float _carriedBoxReachDistance = 400f;
        [Export] private float _carriedBoxLiftTime = 0.2f;
        [Export] private float _carriedBoxDropTime = 0.2f;

        #region Paths
        [Export] private NodePath _carryingDetectorPath = null;
        [Export] private NodePath _aboveHeadPositionPath = null;
        [Export] private NodePath _boxDropPositionPath = null;
        #endregion
        
        private RayCast2D _ray;

        private CarriableBox _carriedBox = null;
        private bool _isCarryingBox => _carriedBox != null;

        private const PhysicsLayers PUSHABLE_LAYERS = PhysicsLayers.World | PhysicsLayers.CarriableBox;
        private PlayerHorizontalMovement _horizontalMovement;
        private float _horizontalMaxSpeed => _horizontalMovement.MaxSpeed;

        private Position2D _aboveHeadPosition;
        private Position2D _boxDropPosition;

        public override void _Ready()
        {
            base._Ready();
            
            _ray = GetNode<RayCast2D>(_carryingDetectorPath);
            _horizontalMovement = GetSiblingBehaviour<PlayerHorizontalMovement>(BehavioursPos.HorizontalMovement);
            _aboveHeadPosition = GetNode<Position2D>(_aboveHeadPositionPath);
            _boxDropPosition = GetNode<Position2D>(_boxDropPositionPath);
        }

        public override void Run(PlayerCurrentFrameData data)
        {
            CarriableBox box;

            if (TrySeePushableBox(data, out box))
            {
                data.VelocityMult = _pushingVelMult;
                box.ApplyVelocity(new Vector2(data.Velocity.x * data.VelocityMult, 0));
                
                data.NextAnimation = PlayerAnimation.Pushing;
            }
            else if (InputManager.IsActionJustPressed(InputButton.Carry) && TrySeeCarriableBox(data, out box))
            {
                box.SetCarried(true);

                box.Reparent(newParent : this);

                _carriedBox = box;

                var tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(_carriedBox, "global_position", _aboveHeadPosition.GlobalPosition, _carriedBoxLiftTime);
            }
            else if (InputManager.IsActionJustPressed(InputButton.Carry) && _isCarryingBox)
            {
                var tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(_carriedBox, "global_position", _boxDropPosition.GlobalPosition, 0.4f);
                tween.TweenCallback(_carriedBox, nameof(CarriableBox.SetCarried), new Godot.Collections.Array(false));
                tween.TweenCallback(_carriedBox, nameof(CarriableBox.SetMovingParent), new Godot.Collections.Array(GetPlayer().GetMovingParentDetector().CurrentMovingParent));
                _carriedBox = null;
            }
            else if (_isCarryingBox)
            {
                _carriedBox.GlobalPosition += (_aboveHeadPosition.GlobalPosition - _carriedBox.GlobalPosition) * 0.85f;
                //_carriedBox.GlobalPosition = _carriedBox.GlobalPosition.MoveToward(_aboveHeadPosition.GlobalPosition, _carriedBoxVelocity * data.Delta);
            }
        }

        // this method may have an issue with evaluating if the player is on the floor, jumping once seems to fix it temporarily.
        private bool TrySeePushableBox(PlayerCurrentFrameData data, out CarriableBox box)
        {
            box = null;
			return (data.Velocity.x != 0f &&
                    IsOnFloor() && 
                    _ray.TryCollideRay<CarriableBox>(out box, toLocal : new Vector2(_horizontalMaxSpeed * data.Delta, 0), mask : PhysicsLayers.CarriableBox) &&
                    box.CollisionLayer == PUSHABLE_LAYERS) && box.CanBePushed(); 
        }

        private bool TrySeeCarriableBox(PlayerCurrentFrameData data, out CarriableBox box)
        {
            box = null;
            return (IsOnFloor() && 
                    _ray.TryCollideRay<CarriableBox>(out box, toLocal : new Vector2(_carriedBoxReachDistance, 0), mask : PhysicsLayers.CarriableBox) &&
                    box.CanBeLifted()
                    );
        }
    }
}