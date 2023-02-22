using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface IIconDisplay
{
	void Appear(Node parent, Vector2 globalPosition, Texture icon, (int width, int height)? overrideSize = null);
	void Disappear();
	Texture CurrentIcon();
}