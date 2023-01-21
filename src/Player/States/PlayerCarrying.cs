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
        #region Paths
        [Export] private NodePath _carryingDetectorPath = null;
        [Export] private NodePath _aboveHeadPositionPath = null;
        [Export] private NodePath _boxDropPositionPath = null;
        #endregion
        
        private RayCast2D _ray;

        private CarriableBox _carriedBox = null;
        private bool _isCarryingBox => _carriedBox != null;

        public override void _Ready()
        {
            base._Ready();
            
            _ray = GetNode<RayCast2D>(_carryingDetectorPath);
            
        }

        public override void Run(PlayerCurrentFrameData data)
        {
            bool isPressed = InputManager.IsActionJustPressed(Button.Carry);
            
            if (!_isCarryingBox && isPressed)
            {
                if (_ray.TryCollideRay(out CarriableBox box, mask : PhysicsLayers.CarriableBox))
                {
                    
                }
            }
            else if (_isCarryingBox && isPressed)
            {
                if (!_ray.CastRay(mask : PhysicsLayers.World))
                {

                }
            }
        }
    }
}