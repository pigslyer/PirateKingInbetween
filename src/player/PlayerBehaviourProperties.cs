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
		
		[Export(hintString: "If |velocity| < this, Idle is used, otherwise movement.")] 
		public float MovementIdleEpsilon = 0.2f;
	}
}