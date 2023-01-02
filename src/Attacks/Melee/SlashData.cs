using Godot;
using System;
using MonoCustomResourceRegistry;

namespace PirateInBetween.Game
{
	[RegisteredType(nameof(SlashData))]
	public class SlashData : AttackData
	{
		[Export] public SlashType SlashType { get; private set; }
		[Export] public float StartupTime { get; private set; } = 0f;
		[Export] public float MidPause { get; private set; } = 0f;
		[Export] public float DownTime { get; private set; } = 0f;
		[Export] public float PostPause { get; private set; } = 0f;
	}
}