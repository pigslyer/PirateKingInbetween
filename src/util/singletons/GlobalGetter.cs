using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Pigslyer.PirateKingInbetween.Player;

namespace Pigslyer.PirateKingInbetween.Util.Singletons
{
	public class GlobalGetter : SingletonReal<GlobalGetter>
	{
		private GlobalGetterWrapper<PlayerController> _player = new();
		public static PlayerController Player
		{
			set => Instance._player.Instance = value;
			get => Instance._player.Instance;
		}


		private class GlobalGetterWrapper<T> where T : class
		{
			private WeakReference<T>? _instance = null;
			public T Instance
			{
				set
				{
					if (_instance == null || !_instance.TryGetTarget(out var _))
					{
						_instance = new(value);
					}
				}
				get
				{
					if (_instance?.TryGetTarget(out var ret) ?? false)
					{
						return ret;
					}

					throw new Exception($"No registered instance of {typeof(T)} exists.");
				}
			}
		}
	}
}