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
		// genuinely surprised Godot doesn't throw a fit, this is for interpolation.
		private static readonly SceneTreeTween _tween = new SceneTreeTween();

		private static readonly (Combo combo, ComboAttr attr)[] _combos = ReflectionHelper.GetInstancesWithAttribute<Combo, ComboAttr>();
		public static ReadOnlyCollection<(Combo combo, ComboAttr attr)> Combos => new ReadOnlyCollection<(Combo combo, ComboAttr attr)>(_combos);

		private List<TaskCompletionSource<ICombatFrameData>> _tasks = new List<TaskCompletionSource<ICombatFrameData>>();

		private bool _executingCombo = false;
		public async Task ExecuteCombo(IComboExecutor executor, ICombatFrameData startingData)
		{

			_executingCombo = true;

			await BeginCombo(executor, startingData);

			_executingCombo = false;
		}

		public void GiveFrameData(ICombatFrameData data)
		{
			if (!_executingCombo)
			{
				throw new InvalidOperationException("I swear to god");
			}

			IEnumerable<TaskCompletionSource<ICombatFrameData>> tasks = _tasks.ToArray();
			_tasks.Clear();

			foreach (var task in tasks)
			{
				task.SetResult(data);
			}
		}

		protected async Task<ICombatFrameData> GetNextFrameData()
		{
			_tasks.Add(new TaskCompletionSource<ICombatFrameData>());
			return await _tasks.Last().Task;
		}

		/// <summary>
		/// The argumnets of what are: elapsed time, delta time, total time.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="what"></param>
		/// <param name="startingData"></param>
		/// <returns></returns>
		protected async Task DoFor(float time, Action<float, float, float, ICombatFrameData> what, ICombatFrameData startingData)
		{
			float t = 0f;
			ICombatFrameData data = startingData ?? await GetNextFrameData();

			while (t < time)
			{
				what(t, Mathf.Max(data.Delta, time - t), time, data);

				t += data.Delta;

				data = await GetNextFrameData();
			}
		}


		protected static T GetCubicInterp<T>(T init, T delta, float elapsed, float duration)
		{
			return (T)_tween.InterpolateValue(init, delta, elapsed, duration, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
		}

		protected abstract Task BeginCombo(IComboExecutor executor, ICombatFrameData startingData);
	}
}