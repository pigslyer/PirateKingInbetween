using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Player
{
	public partial class PlayerBehaviourProperties : Node
	{

		[ExportGroup("Movement")]

		[Export] public float AccelerationRate { get; private set; } = 400.0f;
		[Export] public float DeaccelerationRate { get; private set; } = 1200.0f;
		[Export] public float MaximumVelocity { get; private set; } = 200.0f;

		[ExportGroup("Jumping")]
		[Export] public float Gravity { get; private set; } = 40.0f;
		[Export] public float GravityPushDownAdditional { get; private set; } = 10.0f;

		[Export] public float JumpVelocity { get; private set; } = -200.0f;
		[Export] public float JumpPushupAdditional { get; private set; } = 40.0f;


		[ExportGroup("Model")]	
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; } = null!;
		
		[Export] public float MovementIdleEpsilon = 0.2f;
		[Export] public bool UsingWoodLeg = false;


		[ExportGroup("Camera settings")]
		[Export] public Camera2DController CameraController { get; private set; } = null!;
		[Export] public float CameraOffset { get; private set; } = 40.0f;

		[Export] public float CameraBobStrength { get; private set; } = 1.5f;
		[Export] public float CameraBobSpeed { get; private set; } = 0.625f;

		[ExportGroup("Shooting data")]
		[Export] public Marker2D ShootFromPosition { get; private set; } = null!; 
		[Export] public float ShootTotalWaitTime { get; private set; } = 1.5f;
		[Export] public float ShootAfterTime { get; private set; } = 0.6f;
		[Export] public float ShootBulletVelocity { get; private set; } = 200.0f;

	}
}