using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue.Tree.Writer
{
	public class Manager : Control
	{
		#region Paths
		[Export] private NodePath __quitHandlerPath;

		#endregion

		private QuitHandler _quitHandler;
		private QuitRequester _quitState = 0;

		public override void _Ready()
		{
			base._Ready();

			_quitHandler = GetNode<QuitHandler>(__quitHandlerPath);
		}

		public void SetQuitAllowed(QuitRequester requester, bool state)
		{
			if (state)
			{
				_quitState &= ~requester;
			}
			else 
			{
				_quitState |= requester;
			}

			_quitHandler.SetPopupEnabled(_quitState != 0);
		}

		[Flags] public enum QuitRequester
		{
			Writer = 1,
			CharacterManager = 2,
		};
	}
}