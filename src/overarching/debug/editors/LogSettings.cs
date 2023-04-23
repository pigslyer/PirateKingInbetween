using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Overarching.Debug.Editors
{
	public class LogSettings : IDebugPanel
	{
		public DebugContainer.DebugConfiguration GetPanelData()
		{
			List<DebugContainer.DebugUIElement> uiElements = new();

			VBoxContainer flags = new();
			uiElements.Add(new(
				Title: "Flags",
				Control: flags
			));

			foreach (var value in Enum.GetValues<Log.Types>())
			{
				// lambda has to capture these variable instances
				Log.Types curValue = value;
				CheckBox button = new()
				{
					Text = value.ToString(),
				};

				button.Toggled += state =>
				{
					if (state)
					{
						Log.EnabledTypes |= curValue;
					}
					else
					{
						Log.EnabledTypes &= ~curValue;
					}
				};

				Log.OnMessageLogFiltersChanged += filter =>
				{
					button.SetPressedNoSignal((filter & curValue) != 0);
				};

				button.SetPressedNoSignal((Log.EnabledTypes & curValue) != 0);
				flags.AddChild(button);
			}

			CheckBox allButton = new()
			{
				Text = "All",
			};

			allButton.Toggled += state =>
			{
				Log.EnabledTypes = (Log.Types)(state ? ~0 : 0);
			};

			Log.OnMessageLogFiltersChanged += filter =>
			{
				bool allToggled = true;

				foreach (var value in Enum.GetValues<Log.Types>())
				{
					if ((Log.EnabledTypes & value) == 0)
					{
						allToggled = false;
						break;
					}
				}

				allButton.SetPressedNoSignal(allToggled);
			};

			allButton.SetPressedNoSignal(Enum.GetValues<Log.Types>().All(t => (t & Log.EnabledTypes) != 0));
			flags.AddChild(allButton);

			return new(
				menuTitle: "Log settings",
				UiElements: uiElements
			);
		}
	}
}