using Godot;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Player.Behaviours;

namespace PirateInBetween.Game.Combos
{
	public abstract class Combo
	{
		// genuinely surprised Godot doesn't throw a fit, this is for interpolation.
		private static readonly SceneTreeTween _tween = new SceneTreeTween();

		protected ICombatFrameData CurrentData { get; private set; }
		protected IComboExecutor CurrentExecutor { get; private set; }

		private List<Action> _onFinished = new List<Action>();

		private SyncTaskPool _taskPool = new SyncTaskPool();

		public void ExecuteCombo(IComboExecutor executor, ICombatFrameData startingData)
		{
			CurrentData = startingData;
			CurrentExecutor = executor;
			executor.OnDamageTakenSet(OnDamageTaken);
			
			BeginCombo();
		}

		public bool IsDone() => _taskPool.IsDone();

		public void Stop()
		{
			CurrentExecutor.OnDamageTakenReset();

			CurrentData = null;
			CurrentExecutor = null;
			
			_onFinished.ForEach(action => action());
			_onFinished.Clear();
		}

		protected virtual void OnDamageTaken(ICombatFrameData frameData, DamageData data)
		{ }

		/// <summary>
		/// Performs action when combo finishes. This list is cleared when the combo finishes, meaning registering things before it begins is valid.
		/// </summary>
		/// <param name="what"></param>
		public void OnFinished(Action what) => _onFinished.Add(what);

		public void GiveFrameData(ICombatFrameData data)
		{
			CurrentData = data;
			_taskPool.Run();
		}

		protected static T GetCubicInterp<T>(T init, T delta, float elapsed, float duration)
		{
			return (T)_tween.InterpolateValue(init, delta, elapsed, duration, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
		}

		protected abstract void BeginCombo();


		private float _waitingTime = 0f;

		protected void Wait(float time) => _waitingTime += time;

		protected ComboTask AddTask()
		{
			return new ComboTask(this, _taskPool.Register(() => { }, () => true)).WaitFor(_waitingTime);
		}


		protected class ComboTask
		{
			private readonly Combo _combo;
			private readonly IChainable<(Action, Func<bool>)> _currentLink;
			
			private float _totalTime = 0f;
			private float _elapsedTime = 0f;

			private ComboTask(Combo comboBase, ComboTask prev, Action<ComboTask> whatDo, float time)			
			{
				_combo = comboBase;
				_currentLink = prev._currentLink.Chain((() => whatDo(this), Countdown));
				_totalTime = time;
			}

			public ComboTask(Combo comboBase, IChainable<(Action, Func<bool>)> taskBase)
			{
				_combo = comboBase;
				_currentLink = taskBase;
			}

			public ComboTask DisableDamageFor(FloatInterval time, ComboExecutorDamageTaker area)
			{
				return WaitFor(time.Start)
					.Do(() => _combo.CurrentExecutor.StopTakingDamage(area))
					.WaitFor(time.Delta)
					.Do(() => _combo.CurrentExecutor.TakeDamage(area));
			}

			public ComboTask DealDamageFor(FloatInterval time, ComboExecutorDamageDealers area, DamageAmount amount)
			{
				return WaitFor(time.Start)
					.Do(() => _combo.CurrentExecutor.DealDamage(area, amount))
					.WaitFor(time.Delta)
					.Do(() => _combo.CurrentExecutor.StopDealingDamage(area)
				);
			}

			public ComboTask CubicInterpFor(Vector2 from, Vector2 to, Action<Vector2> setter, float time) => CubicInterpFor<Vector2>(from, to - from, setter, time);
			public ComboTask CubicInterpFor(float from, float to, Action<float> setter, float time) => CubicInterpFor<float>(from, to - from, setter, time);

			public ComboTask CubicInterpFor<T>(T from, T delta, Action<T> setter, float time)
			{
				return DoFor(time, (elapsed, _, total) => setter(GetCubicInterp<T>(from, delta, elapsed, total)));
			}

			public ComboTask Do(Action what) => Generate(c => what(), 0f);

			public ComboTask DoFor(float time, Action<float, float, float> what)
			{
				return Generate(
					whatDo: c => what(c._elapsedTime, Mathf.Min(c._combo.CurrentData.Delta, c._totalTime - c._elapsedTime), c._totalTime),
					time : time
				);
			}

			public ComboTask WaitFor(float time) => time > 0f ? Generate(c => {}, time) : this;

			protected ComboTask Generate(Action<ComboTask> whatDo, float time)
			{
				return new ComboTask(_combo, this, whatDo, time);
			}

			private bool Countdown()
			{
				_elapsedTime += _combo.CurrentData.Delta;
				return _elapsedTime >= _totalTime;
			}
		}
	}
}