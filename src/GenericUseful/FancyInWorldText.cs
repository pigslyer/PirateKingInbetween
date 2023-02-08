using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween
{
	[ScenePath("res://src/GenericUseful/FancyInWorldText.tscn")]
	public class FancyInWorldText : Node2D, ITextDisplay
	{
		const string APPEAR_ANIMATION = "AppearShorter";
		const string DISAPPEAR_ANIMATION = "Disappear";

		#region Paths

		[Export] private NodePath __animationPlayerPath = null;
		[Export] private NodePath __labelPath = null;

		#endregion
		
		private string _text;

		public void Appear(Node parent, Vector2 globalPosition, string text)
		{
			parent.AddChild(this);
			GlobalPosition = globalPosition;
			
			_text = GetNode<Label>(__labelPath).Text = text;
			GetNode<AnimationPlayer>(__animationPlayerPath).Play(APPEAR_ANIMATION);
		}

		public string CurrentText() => _text;

		public async void Disappear()
		{
			AnimationPlayer player = GetNode<AnimationPlayer>(__animationPlayerPath);

			player.Play(DISAPPEAR_ANIMATION);

			await ToSignal(player, "animation_finished");

			QueueFree();
		}
	}
}