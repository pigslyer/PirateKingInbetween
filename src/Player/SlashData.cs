using Godot;
using System;
using MonoCustomResourceRegistry;

namespace PirateInBetween.Player
{
	[RegisteredType(nameof(SlashData))]
	public class SlashData : AttackData
	{
		[Export] public float PrePause { get; private set; } = 0f;
		[Export] public float StartupTime { get; private set; } = 0f;
		[Export] public float MidPause { get; private set; } = 0f;
		[Export] public float DownTime { get; private set; } = 0f;
		[Export] public float PostPause { get; private set; } = 0f;
	}
}