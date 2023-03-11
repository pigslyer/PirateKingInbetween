using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface ICanvasItem
{
	bool Visible { get; set; }

	void Show();
	void Hide();
	
	Color Modulate { get; set; }
}