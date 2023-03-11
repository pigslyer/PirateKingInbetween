using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween
{
	[ScenePath("res://src/GenericUseful/FancyInWorldDisplay.tscn")]
	public class FancyInWorldDisplay : Node2D, ITextDisplay, IIconDisplay
	{
		const string APPEAR_ANIMATION = "AppearShorter";
		const string DISAPPEAR_ANIMATION = "Disappear";

		#region Paths

		[Export] private NodePath __animationPlayerPath = null;
		[Export] private NodePath __labelPath = null;
		[Export] private NodePath __texturePath = null;

		#endregion
		
		private string _text;
		private Texture _texture;

		public void Appear(Node parent, Vector2 globalPosition, string text)
		{
			Setup(parent, globalPosition);

			Label label = GetNode<Label>(__labelPath);
			_text = label.Text = text;
			label.Show();
		}

		public void Appear(Node parent, Vector2 globalPosition, Texture texture, (int width, int height)? overrideSize = null)
		{
			Setup(parent, globalPosition);

			TextureRect rect = GetNode<TextureRect>(__texturePath);
			_texture = rect.Texture = texture;

			(int width, int height) size = overrideSize ?? (texture?.GetWidth() ?? 0, texture?.GetHeight() ?? 0);

			rect.MarginLeft = -size.width/2; rect.MarginTop = -size.height/2; 
			rect.MarginRight = size.width/2; rect.MarginBottom = size.height/2;

			rect.Show();
		}

		private void Setup(Node parent, Vector2 globalPosition)
		{
			parent.AddChild(this);
			GlobalPosition = globalPosition;
			GetNode<AnimationPlayer>(__animationPlayerPath).Play(APPEAR_ANIMATION);
		}

		public string CurrentText() => _text;
		public Texture CurrentIcon() => _texture;

		public async void Disappear()
		{
			AnimationPlayer player = GetNode<AnimationPlayer>(__animationPlayerPath);

			player.Play(DISAPPEAR_ANIMATION);

			await ToSignal(player, "animation_finished");

			QueueFree();
		}
	}
}