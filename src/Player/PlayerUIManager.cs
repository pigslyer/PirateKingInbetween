using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player
{
	public class PlayerUIManager : CanvasLayer
	{
		#region Paths

		[Export] private NodePath __healthBarPath = null;
		[Export] private NodePath __youDiedPopup = null;

		#endregion

		private float _interpolationTime = 0.4f;

		private Range _healthBar;

		public override void _Ready()
		{
			base._Ready();

			_healthBar = GetNode<Range>(__healthBarPath);

			GetNode<AcceptDialog>(__youDiedPopup).Connect("confirmed", this, nameof(OnDiedConfirmed));
		}

		public void UpdateHealth(int newVal, int maxVal, int? prevVal = null)
		{
			_healthBar.MaxValue = maxVal;
		
			var tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);

			var tweener = tween.TweenProperty(_healthBar, "value", (float) newVal, _interpolationTime);

			if (prevVal is int prev)
			{
				tweener.From((double) prevVal / maxVal);
			}

		}

		public void ShowDeathMenu()
		{
			GetTree().Paused = true;
			GetNode<Popup>(__youDiedPopup).Popup_();
		}

		private void OnDiedConfirmed()
		{
			GetTree().Paused = false;
			GetTree().CallDeferred("reload_current_scene");
		}
	}
}