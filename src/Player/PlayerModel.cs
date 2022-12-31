using Godot;
using System;
using System.Threading.Tasks;

namespace PirateInBetween.Player
{
	public class PlayerModel : Node2D
	{
		[Signal] public delegate void OnSlashEnded(); 

#region Paths

		[Export] private NodePath _shootFromPath = null;
		[Export] private NodePath _slashAnimationPlayerPath = null;

#endregion

		private Position2D _shootFrom;
		private AnimationPlayer _slashAnimationPlayer;

		public override void _Ready()
		{
			_shootFrom = GetNode<Position2D>(_shootFromPath);
			_slashAnimationPlayer = GetNode<AnimationPlayer>(_slashAnimationPlayerPath);
		}

		public void SetAnimation(PlayerAnimation state, bool facingRight)
		{
			Scale = new Vector2(facingRight ? 1f : -1f, 1f);
		}

		private SlashData _activeSlashData = null;
		
		public Vector2 GetShootFromPosition() => _shootFrom.GlobalPosition;

		public async void Slash(SlashData data)
		{
			if (_activeSlashData != null) 
				return;
			
			_activeSlashData = data;

			await this.WaitFor(data.PrePause);

			_slashAnimationPlayer.Play("SlashStart", customSpeed : 1f / data.StartupTime);
			
			await this.WaitFor(data.StartupTime + data.MidPause);

			_slashAnimationPlayer.Play("SlashEnd", customSpeed : 1f / data.DownTime);

			await this.WaitFor(data.DownTime + data.PostPause);

			_activeSlashData = null;

			EmitSignal("OnSlashEnded");
		}
	}
}

