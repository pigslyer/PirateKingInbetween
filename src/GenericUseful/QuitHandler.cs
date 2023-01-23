using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class QuitHandler : ConfirmationDialog
{
	[Export] private bool _autoDisableQuit = true;

	public override void _Ready()
	{
		base._Ready();

		if (_autoDisableQuit)
		{
			GetTree().AutoAcceptQuit = false;
		}

		Connect("confirmed", this, nameof(OnConfirmed));
	}

	public override void _Notification(int what)
	{
		base._Notification(what);

		if (what == NotificationWmQuitRequest && !GetTree().AutoAcceptQuit)
		{
			CallDeferred("popup");
		}
	}

	private void OnConfirmed()
	{
		GetTree().Quit();
	}

	public void SetPopupEnabled(bool state) => GetTree().AutoAcceptQuit = !state;
}