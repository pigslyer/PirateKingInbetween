using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Pigslyer.PirateKingInbetween.Util.Behaviour;

namespace Pigslyer.PirateKingInbetween.Player
{
	public abstract class PlayerBehaviour : IBehaviour
	{
		protected interface IPlayerBehaviours
		{
			void SetEnabled(bool state);
			bool IsAnyActive();
		}

		private bool _isInitialized = false;
		private bool _isEnabled = true;

		private BehaviourManager _behaviourManager = null!;
		public BehaviourManager BehaviourManager 
		{ 
			get => _behaviourManager; 
			set => _behaviourManager = value;
		}

		private PlayerActiveBehaviourContainer _currentActiveBehaviour = null!;
		
		protected readonly IFrameData FrameData;
		protected float Delta => FrameData.Delta;
		protected Vector2 Velocity
		{
			get => FrameData.Velocity;
			set => FrameData.Velocity = value;
		}
		protected float VelocityX
		{
			get => Velocity.X;
			set => Velocity = new(value, Velocity.Y);
		}
		protected float VelocityY
		{
			get => Velocity.Y;
			set => Velocity = new(Velocity.X, value);
		}
		protected bool IsActive => _currentActiveBehaviour.Active == this;
		protected float TimeActive = 0.0f;

		protected readonly PlayerBehaviourProperties BehaviourProperties;

		public PlayerBehaviour(PlayerController playerController)
		{
			BehaviourProperties = playerController.BehaviourProperties;
			FrameData = playerController as IFrameData;
		}

		public void RunBehaviour()
		{
			if (_isInitialized)
			{
				if (_isEnabled)
				{
					PassiveBehaviour();
				}

				if (_currentActiveBehaviour.Active == this)
				{
					ActiveBehaviour();
					TimeActive += Delta;
				}
			}
			else
			{
				_isInitialized = true;

				_currentActiveBehaviour = (BehaviourManager.Behaviours[0] == this) ? new() : ((PlayerBehaviour)(BehaviourManager.Behaviours[0]))._currentActiveBehaviour;
				InitializeBehaviour();

			}
		}

		public abstract void InitializeBehaviour();
		public abstract void PassiveBehaviour();
		public abstract void ActiveBehaviour();

		public void SetActive()
		{
			if (_currentActiveBehaviour.Active != null)
			{
				if (_currentActiveBehaviour.Active == this)
				{
					Log.PushWarning(
						Log.Types.PlayerBehaviours,
						$"{GetType()} just attempted to set itself as active twice."
					);
				}
				else
				{
					Log.PushError(
						Log.Types.PlayerBehaviours,
						$"{GetType()} just attempted to set itself as active when another {nameof(PlayerBehaviour)} is already active."
					);
				}
			}
			else
			{
				Log.Print(
					Log.Types.PlayerBehaviours,
					$"{GetType()} has become active."
				);
				_currentActiveBehaviour.Active = this;
			}
		}

		public void ResetActive()
		{
			if (_currentActiveBehaviour.Active != this)
			{
				if (_currentActiveBehaviour.Active == null)
				{
					Log.PushWarning(
						Log.Types.PlayerBehaviours,
						$"{GetType()} needlessly called {nameof(ResetActive)}, no behaviour is active at the moment."
					);
				}
				else
				{
					Log.PushError(
						Log.Types.PlayerBehaviours,
						$"{GetType()} attempted to set other {nameof(PlayerBehaviour)} as inactive!"
					);
				}
			}
			else
			{
				Log.Print(
					Log.Types.PlayerBehaviours, 
					$"{GetType()} has become inactive."
				);
				
				_currentActiveBehaviour.Active = null;
				TimeActive = 0.0f;
			}
		}

		protected bool IsBehaviourActive<T>() where T : PlayerBehaviour
		{
			return typeof(T).IsInstanceOfType(_currentActiveBehaviour.Active);
		}

		protected IPlayerBehaviours GetBehaviours(Type[] includingTypes, bool getAllBut = false)
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException($"Cannot call {nameof(GetBehaviours)} before {nameof(InitializeBehaviour)} has been called!");
			}

			if (includingTypes.Length == 1 && !getAllBut)
			{
				return PlayerBehavioursSingle.Generate(BehaviourManager.Behaviours, includingTypes[0]);
			}

			return PlayerBehaviours.Generate(BehaviourManager.Behaviours, includingTypes, getAllBut);
		}

		private class PlayerBehaviours : IPlayerBehaviours
		{
			private readonly IEnumerable<PlayerBehaviour> _behaviours;

			private PlayerBehaviours(IEnumerable<PlayerBehaviour> behaviours)
			{
				_behaviours = behaviours;
			}

			public static PlayerBehaviours Generate(IEnumerable<IBehaviour> behaviours, IEnumerable<Type> includingTypes, bool getAllBut)
			{
				PlayerBehaviour[] ret = new PlayerBehaviour[includingTypes.Count()];
				int i = 0;

				foreach (Type type in includingTypes)
				{
					foreach (IBehaviour behaviour in behaviours)
					{
						if (type.IsInstanceOfType(behaviour) != getAllBut)
						{
							ret[i++] = (PlayerBehaviour)behaviour;
							break;
						}
					}
				}

				return new(ret);
			}

			public void SetEnabled(bool state)
			{
				foreach (PlayerBehaviour b in _behaviours)
				{
					b._isEnabled = state;
				}
			}

			public bool IsAnyActive()
			{
				foreach (PlayerBehaviour b in _behaviours)
				{
					if (b._currentActiveBehaviour.Active == b)
					{
						return true;
					}
				}

				return false;
			}
		}

		private class PlayerBehavioursSingle : IPlayerBehaviours
		{
			private readonly PlayerBehaviour _behaviour;

			private PlayerBehavioursSingle(PlayerBehaviour behaviour)
			{
				_behaviour = behaviour;
			}

			public static PlayerBehavioursSingle Generate(IEnumerable<IBehaviour> behaviours, Type forType)
			{
				foreach (IBehaviour b in behaviours)
				{
					if (forType.IsInstanceOfType(b))
					{
						return new((PlayerBehaviour) b);
					}
				}

				throw new Exception($"Could not find {nameof(PlayerBehaviour)} of type {forType}!");
			}
			
			public bool IsAnyActive()
			{
				return _behaviour._currentActiveBehaviour.Active == _behaviour;
			}

			public void SetEnabled(bool state)
			{
				_behaviour._isEnabled = state;
			}
		}

		private class PlayerActiveBehaviourContainer
		{
			public PlayerBehaviour? Active = null;
		}
	}
}