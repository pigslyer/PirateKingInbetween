using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Overarching.Debug
{
	[Pigslyer.PirateKingInbetween.Util.Reflection.Path("res://src/overarching/debug/DebugItemScene.tscn")]
	public partial class DebugItemScene : HBoxContainer
	{
		[Export] private Label _title = null!;

		private Control? _debugItem = null;
		public void SetData(string? title, Control debugItem)
		{
			_title.Visible = title != null;
			_title.Text = title;
			
			_debugItem?.QueueFree();
			debugItem.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			debugItem.SizeFlagsVertical = SizeFlags.ExpandFill;
			AddChild(_debugItem = debugItem);

			
		}
	}
}