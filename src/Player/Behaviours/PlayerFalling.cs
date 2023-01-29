using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player.Behaviours
{
    public class PlayerFalling : PlayerBehaviour
    {
        
		// The constant y velocity applied down to the player.
		[Export] public float Gravity { get; private set; } = 800f;

        public override void Run(PlayerCurrentFrameData data)
        {
            data.Velocity.y += Gravity * data.Delta;
        }
    }
}