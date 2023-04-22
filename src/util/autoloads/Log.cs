using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;

using Pigslyer.PirateKingInbetween.Overarching.Debug;

namespace Pigslyer.PirateKingInbetween.Util.Autoloads
{
	public partial class Log : Singleton<Log>
	{
		public partial class LogMessage : RefCounted
		{
			public readonly WarningLevels WarningLevel; 
			public readonly Types LogType; 
			public readonly string Message;

			public LogMessage(WarningLevels warningLevel, Types logType, string message)
			{
				WarningLevel = warningLevel; LogType = logType; Message = message;
			}
		}

		[Signal] public delegate void OnMessageLoggedEventHandler(LogMessage logMessage);
		[Signal] public delegate void OnMessageLogFiltersChangedEventHandler(Types newFilter);

		public enum WarningLevels
		{
			Print,
			Warning,
			Error,
		};

		public enum Types
		{
			Debug = 1,
			PlayerBehaviours = 2,
			Input = 4,
		};

		private Types EnabledTypes = (Types)~0;

		public static void Print(Types source, string str)
		{
			GD.Print(str);
			Instance.EmitSignal(SignalName.OnMessageLogged, new LogMessage(
				message: str,
				logType: source,
				warningLevel: WarningLevels.Print
			));
		}

		public static void PushWarning(Types source, string str)
		{
			GD.PushWarning(str);
			Instance.EmitSignal(SignalName.OnMessageLogged, new LogMessage(
				message: str,
				logType: source,
				warningLevel: WarningLevels.Warning
			));
		}

		public static void PushError(Types source, string str)
		{
			GD.PushError(WarningLevels.Error);
			Instance.EmitSignal(SignalName.OnMessageLogged, new LogMessage(
				message: str,
				logType: source,
				warningLevel: WarningLevels.Error
			));
		}

		public static DebugContainer.DebugConfiguration GetLogDisplay()
		{
			Color GetColor(LogMessage msg) => msg.WarningLevel switch
			{
				WarningLevels.Error => Colors.Red,
				WarningLevels.Warning => Colors.Yellow,
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

			void AddMessage(LogMessage msg)
			{
				label.PushColor(GetColor(msg));
				label.AddText($"[{msg.LogType.ToString()}]: {msg.Message}");
				label.Pop();
			}

			uiElements.Add(new(null, scroll));
			scroll.AddChild(label);

			List<LogMessage> messages = new();

			Instance.OnMessageLogged += msg =>
			{
				messages.Add(msg);

				if ((Instance.EnabledTypes & msg.LogType) != 0)
				{
					AddMessage(msg);
				}
			};

			Instance.OnMessageLogFiltersChanged += filters =>
			{
				label.Clear();
				
				foreach (LogMessage msg in messages)
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

		public static DebugContainer.DebugConfiguration GetLogSettings()
		{
			List<DebugContainer.DebugUIElement> uiElements = new();

			VBoxContainer flags = new();
			uiElements.Add(new(
				Title: "Flags",
				Control: flags 
			));

			foreach (var value in Enum.GetValues<Types>())
			{
				// lambda has to capture these variable instances
				Types curValue = value;
				CheckBox button = new()
				{
					Text = value.ToString(),
				};

				button.Toggled += state =>
				{
					if (state)
					{
						Instance.EnabledTypes |= curValue;
					}
					else
					{
						Instance.EnabledTypes &= ~curValue;
					}
					Instance.EmitSignal(SignalName.OnMessageLogFiltersChanged, (uint) Instance.EnabledTypes);
				};

				Instance.OnMessageLogFiltersChanged += filter =>
				{
					button.SetPressedNoSignal((filter & curValue) != 0);
				};

				flags.AddChild(button);
			}

			CheckBox allButton = new()
			{
				Text = "All",
			};

			allButton.Toggled += state =>
			{
				Instance.EnabledTypes = (Types)(state ? ~0 : 0);
				Instance.EmitSignal(SignalName.OnMessageLogFiltersChanged, (uint)Instance.EnabledTypes);
			};

			Instance.OnMessageLogFiltersChanged += filter =>
			{
				bool allToggled = true;

				foreach (var value in Enum.GetValues<Types>())
				{
					if ((Instance.EnabledTypes & value) == 0)
					{
						allToggled = false;
						break;
					}
				}

				allButton.SetPressedNoSignal(allToggled);
			};

			flags.AddChild(allButton);

			Instance.EmitSignal(SignalName.OnMessageLogFiltersChanged, (uint)Instance.EnabledTypes);

			return new(
				menuTitle: "Log settings",
				UiElements: uiElements
			);
		}
	}
}