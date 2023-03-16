using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface INode2D : ICanvasItem
{
	Vector2 Position { get; set; }
	Vector2 GlobalPosition { get; set; }
	
}