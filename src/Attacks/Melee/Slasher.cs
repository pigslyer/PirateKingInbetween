using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace PirateInBetween.Game
{
	public class Slasher : Node2D
	{
		[Export(PropertyHint.Layers2dPhysics)] uint _hitsLayers = 0;
		[Export] List<SlashData> _slashes = new List<SlashData>();

#region Paths
		[Export] NodePath _slashAnimationPlayerPath = null;
		[Export] NodePath _hitterPath = null;
#endregion

		private AnimationPlayer _slashAnimationPlayer;
		private Hitter _hitter;

		private SlashData _activeSlashData = null;

		public override void _Ready()
		{
			_slashAnimationPlayer = GetNode<AnimationPlayer>(_slashAnimationPlayerPath);
			_hitter = GetNode<Hitter>(_hitterPath);
			
			_hitter.SetBaseMask((PhysicsLayers) _hitsLayers);
		}

		public Task Slash(SlashType type)
		{
			SlashData data = _slashes.Find(s => s.SlashType == type);
			
			if (data == null)
			{
				GD.PushError($"Attempted to play {type}, but this type of slash isn't present in {GetPath()}");
			}

			return Slash(data);
		}

		public async Task Slash(SlashData data)
		{
			if (_activeSlashData != null || data == null)
				return;

			_activeSlashData = data;

			_hitter.HitFor(data.StartupTime + data.MidPause + data.DownTime);

			_slashAnimationPlayer.Play("BasicStart", customSpeed: 1f / data.StartupTime);

			await this.WaitFor(data.StartupTime + data.MidPause);

			_slashAnimationPlayer.Play("BasicEnd", customSpeed: 1f / data.DownTime);

			await this.WaitFor(data.DownTime + data.PostPause);

			_activeSlashData = null;
		}
	}
}