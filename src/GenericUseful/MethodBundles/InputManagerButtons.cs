using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public enum InputButton
{
	[EnumString("mv_right")] MoveRight,
	[EnumString("mv_up")] MoveUp,
	[EnumString("mv_left")] MoveLeft,
	[EnumString("mv_down")] MoveDown,
	[EnumString("attack_melee")] MeleeAttack,
	[EnumString("attack_shoot")] RangedAttack,
	[EnumString("player_carry")] Carry,
	[EnumString("interact")] Interact,
}