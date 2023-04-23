using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Overarching.Debug.Editors
{
	public class LogDisplay : IDebugPanel
	{
		public DebugContainer.DebugConfiguration GetPanelData()
		{
			Color GetColor(Log.LogMessage msg) => msg.WarningLevel switch
			{
				Log.WarningLevels.Error => Colors.Red,
				Log.WarningLevels.Warning => Colors.Yellow,
				_ => Colors.White
			};

			const int LOG_HEIGHT = 200;

			List<DebugContainer.DebugUIElement> uiElements = new();

			ScrollContainer scroll = new()
			{
				CustomMinimumSize = new Vector2(0, LOG_HEIGHT),
			};

			RichTextLabel label = new()
			{
				BbcodeEnabled = true,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			void AddMessage(Log.LogMessage msg)
			{
				label.PushColor(GetColor(msg));
				label.AddText($"[{msg.LogType.ToString()}]: {msg.Message}");
				label.Pop();
			}

			uiElements.Add(new(null, scroll));
			scroll.AddChild(label);

			List<Log.LogMessage> messages = new();

			Log.OnMessageLogged += msg =>
			{
				messages.Add(msg);

				if ((Log.EnabledTypes & msg.LogType) != 0)
				{
					AddMessage(msg);
				}
			};

			Log.OnMessageLogFiltersChanged += filters =>
			{
				label.Clear();

				foreach (var msg in messages)
				{
					if ((filters & msg.LogType) != 0)
					{
						AddMessage(msg);
					}
				}
			};

			return new(
				menuTitle: "Log",
				UiElements: uiElements
			);
		}
	}
}