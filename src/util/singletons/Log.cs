using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;

using Pigslyer.PirateKingInbetween.Overarching.Debug;

namespace Pigslyer.PirateKingInbetween.Util.Singletons
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

		[Signal] public delegate void _onMessageLoggedEventHandler(LogMessage logMessage);
		[Signal] public delegate void _onMessageLogFiltersChangedEventHandler(Types newFilter);

		public static event _onMessageLogFiltersChangedEventHandler OnMessageLogFiltersChanged
		{
			add => Instance._onMessageLogFiltersChanged += value;
			remove => Instance._onMessageLogFiltersChanged -= value;
		}

		public static event _onMessageLoggedEventHandler OnMessageLogged
		{
			add => Instance._onMessageLogged += value;
			remove => Instance._onMessageLogged -= value;
		}

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

		private Types _enabledTypes = (Types)~0;
		public static Types EnabledTypes
		{
			get => Instance._enabledTypes;
			set
			{	
				Instance._enabledTypes = value;
				Instance.EmitSignal(SignalName._onMessageLogFiltersChanged, (uint)Instance._enabledTypes);
			}
		}

		public static void Print(Types source, string str)
		{
			GD.Print(str);
			Instance.EmitSignal(SignalName._onMessageLogged, new LogMessage(
				message: str,
				logType: source,
				warningLevel: WarningLevels.Print
			));
		}

		public static void PushWarning(Types source, string str)
		{
			GD.PushWarning(str);
			Instance.EmitSignal(SignalName._onMessageLogged, new LogMessage(
				message: str,
				logType: source,
				warningLevel: WarningLevels.Warning
			));
		}

		public static void PushError(Types source, string str)
		{
			GD.PushError(WarningLevels.Error);
			Instance.EmitSignal(SignalName._onMessageLogged, new LogMessage(
				message: str,
				logType: source,
				warningLevel: WarningLevels.Error
			));
		}
	}
}