using System.Collections.Specialized;
using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Player.Actions;

namespace PirateInBetween.Game.Player.Behaviours
{
    public class PlayerCarrying : PlayerBehaviour
    {
        [Export] private float _pushingVelMult = 0.3f;

        [Export] private float _carriedBoxReachDistance = 400f;
        [Export] private float _carriedBoxLiftTime = 0.2f;
        [Export] private float _carriedBoxDropTime = 0.2f;
        [Export] private float _carryingLerpFactor = 0.85f;

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
        private PlayerAnimationSelector _animationSelector;
        private float _horizontalMaxSpeed => _horizontalMovement.MaxSpeed;

        private Position2D _aboveHeadPosition;
        private Position2D _boxDropPosition;

        private const Behaviours _onPickUpBehaviours = Behaviours.Default & ~(Behaviours.MeleeAttack | Behaviours.RangedAttack);
        private const Behaviours _onDropBehaviours = Behaviours.Default;

        public override void _Ready()
        {
            base._Ready();
            
            _ray = GetNode<RayCast2D>(_carryingDetectorPath);
            _horizontalMovement = GetSiblingBehaviour<PlayerHorizontalMovement>(BehavioursPos.HorizontalMovement);
            _animationSelector = GetSiblingBehaviour<PlayerAnimationSelector>(BehavioursPos.AnimationSelector);
            _aboveHeadPosition = GetNode<Position2D>(_aboveHeadPositionPath);
            _boxDropPosition = GetNode<Position2D>(_boxDropPositionPath);
        }

        public override void Run(PlayerCurrentFrameData data)
        {
            CarriableBox box;

            if (IsOnFloor() && TrySeePushableBox(data, out box))
            {
                data.VelocityMult = _pushingVelMult;
                box.ApplyVelocity(new Vector2(data.Velocity.x * data.VelocityMult, 0));
                
                data.CurrentAction = PlayerAnimation.Pushing;
            }
            else if (IsOnFloor() && CanChangeActive && !_isCarryingBox && TrySeeCarriableBox(data, out box))
            {
                var anim = _animationSelector.GetDefaultAnimation(data);

                data.CurrentAction = new ActionLookingAt(anim.Animation, "Pick up box");
                data.FacingRight = anim.FacingRight;

                if (InputManager.IsActionJustPressed(InputButton.Carry))
                {
					box.SetCarried(true);
					_carriedBox = box;

					CarryBoxAnimation(
							box: box,
							getToPosition: () => _aboveHeadPosition.GlobalPosition,
							time: _carriedBoxLiftTime,
							onFinishedEnabled: _onPickUpBehaviours,
							onEnd: () => _carriedBox.Reparent2D(newParent: this)
					);
					data.Velocity = Vector2.Zero;
				}
            }
            else if (IsOnFloor() && CanChangeActive && _isCarryingBox && InputManager.IsActionJustPressed(InputButton.Carry))
            {
                _carriedBox.SetMovingParent(GetPlayer().GetMovingParentDetector().CurrentMovingParent);
                CarriableBox temp = _carriedBox;

                CarryBoxAnimation(
                        box: _carriedBox,
                        getToPosition: () => _boxDropPosition.GlobalPosition,
                        time: _carriedBoxDropTime,
                        onFinishedEnabled: _onDropBehaviours,
                        onEnd: () => temp.SetCarried(false)
                );

                data.Velocity = Vector2.Zero;
                _carriedBox = null;
            }
            else if (_isCarryingBox)
            {
				var anim = _animationSelector.GetDefaultAnimation(data);
				data.CurrentAction = new ActionLookingAt(anim.Animation, "Drop box");
				data.FacingRight = anim.FacingRight;
                

                _carriedBox.GlobalPosition = _carriedBox.GlobalPosition.LinearInterpolate(_aboveHeadPosition.GlobalPosition, _carryingLerpFactor);
            }
        }

        private bool TrySeePushableBox(PlayerCurrentFrameData data, out CarriableBox box)
        {
            box = null;
			return (data.Velocity.x != 0f &&
                    _ray.TryCollideRay<CarriableBox>(out box, toLocal : new Vector2(_horizontalMaxSpeed * data.Delta, 0), mask : PhysicsLayers.CarriableBox) &&
                    box.CollisionLayer == PUSHABLE_LAYERS) && box.CanBePushed(); 
        }

        private bool TrySeeCarriableBox(PlayerCurrentFrameData data, out CarriableBox box)
        {
            box = null;
            return (_ray.TryCollideRay<CarriableBox>(out box, toLocal : new Vector2(_carriedBoxReachDistance, 0), mask : PhysicsLayers.CarriableBox) &&
                    box.CanBeLifted()
                    );
        }

        private async Task CarryBoxAnimation(CarriableBox box, Func<Vector2> getToPosition, float time, Behaviours onFinishedEnabled, Action onEnd = null)
        {
            SetBehaviourChangesDisabled(true);
            // disable everything but <see cref="AnimationSelector"/> so the player can't do any funny business.
            SetBehavioursEnabled(Behaviours.Default & ~Behaviours.AnimationSelector, false);

            Vector2 pos;
            float length, delta;

            while (time > 0f)
            {
                pos = getToPosition();
                length = box.GlobalPosition.DistanceTo(pos);
                delta = GetProcessDeltaTime();

                box.GlobalPosition = box.GlobalPosition.MoveToward(pos, length / time * delta);
                time -= delta;

                await this.AwaitIdle();
            }

            onEnd?.Invoke();

            SetBehaviourChangesDisabled(false);
            SetBehavioursEnabled(onFinishedEnabled, true);
        }
    }
}