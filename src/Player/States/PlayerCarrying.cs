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

        public override void _Ready()
        {
            base._Ready();
            
            _ray = GetNode<RayCast2D>(_carryingDetectorPath);
            _horizontalMovement = GetSiblingBehaviour<PlayerHorizontalMovement>(BehavioursPos.HorizontalMovement);
        }

        public override void Run(PlayerCurrentFrameData data)
        {
            if (TrySeePushableBox(data, out var box))
            {
                data.VelocityMult = _pushingVelMult;
                box.ApplyVelocity(new Vector2(data.Velocity.x * data.VelocityMult, 0));
                
                data.NextAnimation = PlayerAnimation.Pushing;
            }
        }

        // this method may have an issue with evaluating if the player is on the floor, jumping once seems to fix it temporarily.
        private bool TrySeePushableBox(PlayerCurrentFrameData data, out CarriableBox box)
        {
            box = null;
			return (data.Velocity.x != 0f &&
                    IsOnFloor() && 
                    _ray.TryCollideRay<CarriableBox>(out box, toLocal : new Vector2(_horizontalMaxSpeed * data.Delta, 0), mask : PhysicsLayers.CarriableBox) &&
                    box.CollisionLayer == PUSHABLE_LAYERS); 
        }
    }
}