using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Player.Behaviours;

namespace PirateInBetween.Game.Combos
{
	public abstract class Combo
	{
		protected class TaskData
		{ }

		// genuinely surprised Godot doesn't throw a fit, this is for interpolation.
		private static readonly SceneTreeTween _tween = new SceneTreeTween();

		private static readonly (Combo combo, ComboAttr attr)[] _combos = ReflectionHelper.GetInstancesWithAttribute<Combo, ComboAttr>();
		public static ReadOnlyCollection<(Combo combo, ComboAttr attr)> Combos => new ReadOnlyCollection<(Combo combo, ComboAttr attr)>(_combos);

		protected ICombatFrameData CurrentData { get; private set; }
		protected IComboExecutor CurrentExecutor { get; private set; }
		private List<TaskCompletionSource<TaskData>> _tasks = new List<TaskCompletionSource<TaskData>>();

		private TaskCompletionSource<float> _comboFinishTask = new TaskCompletionSource<float>();

		private bool _executingCombo = false;
		private List<Action> _onFinished = new List<Action>();

		public async Task ExecuteCombo(IComboExecutor executor, ICombatFrameData startingData)
		{
			_executingCombo = true;
			CurrentData = startingData;
			CurrentExecutor = executor;

			await BeginCombo(executor);

			CurrentData = null;
			CurrentExecutor = null;

			_onFinished.ForEach(action => action());
			_onFinished.Clear();

			_executingCombo = false;

		}

		/// <summary>
		/// Performs action when combo finishes. This list is cleared when the combo finishes, meaning registering things before it begins is valid.
		/// </summary>
		/// <param name="what"></param>
		public void OnFinished(Action what) => _onFinished.Add(what);

		public void GiveFrameData(ICombatFrameData data)
		{
			if (!_executingCombo)
			{
				throw new InvalidOperationException("I swear to god");
			}

			CurrentData = data;

			Update();
		}

		private void Update()
		{
			IEnumerable<TaskCompletionSource<TaskData>> tasks = _tasks.ToArray();
			_tasks.Clear();

			foreach (var task in tasks)
			{
				task.SetResult(null);
			}

		}

		protected async Task<TaskData> AwaitNextFrameData()
		{
			_tasks.Add(new TaskCompletionSource<TaskData>());
			return await _tasks.Last().Task;
		}

		/// <summary>
		/// The argumnets of what are: elapsed time, delta time, total time.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="what"></param>
		/// <param name="startingData"></param>
		/// <returns></returns>
		protected async Task DoFor(float time, Action<float, float, float> what)
		{
			float t = 0f;
			
			while (t < time)
			{
				what(t, Mathf.Max(CurrentData.Delta, time - t), time);

				t += CurrentData.Delta;

				await AwaitNextFrameData();
			}
		}

		protected async Task WaitFor(float time) => await DoFor(time, (a, b, c) => {});

		protected async Task DealDamageFor(FloatInterval time, ComboExecutorDamageDealers area, DamageData data)
		{
			await WaitFor(time.Start);
			
			CurrentExecutor.DealDamage(area, data);
			
			await WaitFor(time.Delta);

			CurrentExecutor.StopDealingDamage(area);
		}

		/// <summary>
		/// Essentially an overload of <see cref="DoFor"/> which directly cubically interpolates the given value and sets it to the target.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="delta"></param>
		/// <param name="setter"></param>
		/// <param name="time"></param>
		/// <param name="startingData"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected async Task CubicInterpFor<T>(T from, T delta, Action<T> setter, float time)
		{
			float t = 0f;

			while (t < time)
			{
				setter(GetCubicInterp<T>(from, delta, t, time));

				t += CurrentData.Delta;
				await AwaitNextFrameData();
			}
		}

		protected async Task CubicInterpFor(Vector2 from, Vector2 to, Action<Vector2> setter, float time) => await CubicInterpFor<Vector2>(from, to - from, setter, time);
		protected async Task CubicInterpFor(float from, float to, Action<float> setter, float time) => await CubicInterpFor<float>(from, to - from, setter, time);

		protected static T GetCubicInterp<T>(T init, T delta, float elapsed, float duration)
		{
			return (T)_tween.InterpolateValue(init, delta, elapsed, duration, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
		}

		protected abstract Task BeginCombo(IComboExecutor executor);
	}
}