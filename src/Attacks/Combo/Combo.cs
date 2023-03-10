using System.IO;
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

		public delegate DamageReaction OnHitReaction();
		protected delegate void DoForStatement(float elapsed, float delta, float total);
		
		protected ICombatFrameData CurrentData { get; private set; }
		protected IComboExecutor CurrentExecutor { get; private set; }

		private List<Action> _onFinished = new List<Action>();

		private SyncTaskPool _taskPool = new SyncTaskPool();
		private bool _isExecuting = false;
		private bool _isProcessing = false;
		private bool _preemptiveStop = false;

		public void ExecuteCombo(IComboExecutor executor, ICombatFrameData startingData)
		{
			_isProcessing = _preemptiveStop = false;
			_isExecuting = true;
			CurrentData = startingData;
			CurrentExecutor = executor;
			executor.OnDamageTakenSet(OnDamageTaken);
			
			BeginCombo();
		}

		public bool IsDone() => _taskPool.IsDone();


		/// <summary>
		/// Stops execution of current combo, clears all data and calls all actions registered in <see cref="OnFinished(Action)"/>.
		/// Safe to be called from a <see cref="ComboTask"/>.
		/// </summary>
		public void Stop()
		{
			if (_isProcessing)
			{
				_preemptiveStop = true;
				return;
			}
 
			if (!_isExecuting)
			{
				return;
			}

			CurrentExecutor.OnDamageTakenSet(CurrentExecutor.OnDamageTakenDefault());

			_taskPool.Clear();

			CurrentData = null;
			CurrentExecutor = null;
			
			_onFinished.ForEach(action => action());
			_onFinished.Clear();

			_isExecuting = false;
		}

		protected virtual DamageReaction OnDamageTaken()
		{
			return CurrentExecutor.OnDamageTakenDefault().Invoke();
		}

		/// <summary>
		/// Performs action when combo finishes. This list is cleared when the combo finishes, meaning registering things before it begins is valid.
		/// </summary>
		/// <param name="what"></param>
		public void OnFinished(Action what) => _onFinished.Add(what);

		public void GiveFrameData(ICombatFrameData data)
		{
			_isProcessing = true;
			CurrentData = data;

			_taskPool.Run();

			_isProcessing = false;

			if (_preemptiveStop)
			{
				Stop();
			}
		}

		protected static T GetInterp<T>(T init, T delta, float elapsed, float duration, Tween.TransitionType trans, Tween.EaseType ease)
		{
			return (T)_tween.InterpolateValue(init, delta, elapsed, duration, trans, ease);
		}

		protected abstract void BeginCombo();


		private float _waitingTime = 0f;

		protected void EnableHorizontalControl(FloatInterval when, float accel, float maxVelocity)
		{
			float velocity = 0f;

			AddTask().WaitFor(when.Start).DoFor(
				when.Delta,
				(float elapsed, float delta, float total) =>
				{
					if (CurrentData.IsMoving())
					{
						if (CurrentData.IsGoingBackwards())
						{
							CurrentData.SwitchDirection();
							velocity = 0f;
						}

						velocity = Mathf.Min(velocity + accel * CurrentData.Delta, maxVelocity);
					}
					else
					{
						velocity = 0f;
					}

					CurrentData.Velocity = new Vector2(velocity * CurrentData.GetDirection(), CurrentData.Velocity.y);
				}
			);
		}

		protected void Wait(float time) => _waitingTime += time;

		protected ComboTask AddTask()
		{
			return new ComboTask(this, _taskPool.Register(() => { }, () => true)).WaitFor(_waitingTime);
		}

		/// <summary>
		/// Represents 1 operation that a combo requests, be that waiting, interpolation or what have you.
		/// </summary>
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

			public ComboTask DisableDamageFor(FloatInterval time, DamageTakerTargetArea area)
			{
				return WaitFor(time.Start)
					.Do(() => _combo.CurrentExecutor.StopTakingDamage(area))
					.WaitFor(time.Delta)
					.Do(() => _combo.CurrentExecutor.TakeDamage(area));
			}

			public ComboTask DealDamageFor(DamageInstance damage)
			{
				return WaitFor(damage.ValidInterval.Start)
					.Do(() => _combo.CurrentExecutor.DealDamage(damage.TargetArea, damage.ToData(_combo.CurrentData)))
					.WaitFor(damage.ValidInterval.Delta)
					.Do(() => _combo.CurrentExecutor.StopDealingDamage(damage.TargetArea)
				);
			}

			public ComboTask PlayFor(Animations animation, float time)
			{
				return DoFor(
					time: time,
					what: (elapsed, _, total) => _combo.CurrentData.Anim = new CombatAnimation(animation, elapsed / total)
				);
			}

			public ComboTask InterpFor(Vector2 from, Vector2 to, Action<Vector2> setter, float time, Tween.TransitionType trans = Tween.TransitionType.Cubic, Tween.EaseType ease = Tween.EaseType.Out) => InterpFor<Vector2>(from, to - from, setter, time, trans, ease);
			public ComboTask InterpFor(float from, float to, Action<float> setter, float time, Tween.TransitionType trans = Tween.TransitionType.Cubic, Tween.EaseType ease = Tween.EaseType.Out) => InterpFor<float>(from, to - from, setter, time, trans, ease);

			public ComboTask InterpFor<T>(T from, T delta, Action<T> setter, float time, Tween.TransitionType trans = Tween.TransitionType.Cubic, Tween.EaseType ease = Tween.EaseType.Out)
			{
				return DoFor(time, (elapsed, _, total) => setter(GetInterp<T>(from, delta, elapsed, total, trans, ease)));
			}

			public ComboTask Do(Action what) => Generate(c => what(), 0f);

			public ComboTask DoIf(float checkFor, Func<bool> ifWhat, Action whatDo)
			{
				bool hasDone = false;

				return Generate(
					c =>
					{
						if (!hasDone && ifWhat())
						{
							whatDo();
							hasDone = true;
						}
					}, 
					time: checkFor);
			}

			public ComboTask DoFor(float time, DoForStatement what)
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

		protected class DamageInstance
		{
			public readonly DamageAmount Damage;
			public readonly Func<Vector2> KnockbackDirection;
			public readonly FloatInterval ValidInterval;
			public readonly DamageDealerTargettingArea TargetArea;

			public DamageInstance(DamageAmount damageAmount, FloatInterval validInterval, DamageDealerTargettingArea targetArea, Func<Vector2> knockbackDirection = null)
			{
				Damage = damageAmount; ValidInterval = validInterval; TargetArea = targetArea; KnockbackDirection = knockbackDirection;
			}

			public DamageData ToData(ICombatFrameData frameData)
			{
				return new DamageData(Damage, KnockbackDirection?.Invoke().FaceForward(frameData) ?? Vector2.Zero);
			}
		}
	}
}