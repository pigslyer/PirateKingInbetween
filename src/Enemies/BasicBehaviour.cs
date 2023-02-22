using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PirateInBetween.Game.Combos;

using PirateInBetween.Game.Enemies.Behaviours;

namespace PirateInBetween.Game.Enemies
{
	/// <summary>
	/// Methods in <see cref="BasicBehaviour"/> should be formatted as having a bool return, which return true
	/// if execution should stop.
	/// </summary>
	public abstract class BasicBehaviour : Node
	{
		#region Paths

		[Export] private NodePath __inFrontCheckPath = null;

		#endregion

		[Export] protected float EnemyWidth { get; private set; } = 16f;		
		protected EnemyController Controller { get; private set; }

		protected EnemyFrameData CurrentData { get; private set; }
		
		private RayCast2D _ray;

		#region Sync tasks

		private SyncTaskPool _syncTasks = new SyncTaskPool();
		
		protected IChainable<BehaviourTask> AddTask(BehaviourTask startingTask)
		{
			return new SyncTaskChain(_syncTasks.Register(startingTask.WhatDo, startingTask.Condition));
		}
		
		protected void ClearTasks() => _syncTasks.Clear();

		protected bool RunTasks()
		{
			_syncTasks.Run();
			return _syncTasks.IsDone();
		}

		private class SyncTaskChain : IChainable<BehaviourTask>
		{
			private IChainable<(Action, Func<bool>)> _link;

			public SyncTaskChain(IChainable<(Action, Func<bool>)> startingLink)
			{
				_link = startingLink;
			}

			public IChainable<BehaviourTask> Chain(BehaviourTask data)
			{
				return new SyncTaskChain(_link.Chain((data.WhatDo, data.Condition)));
			}
		}

		#endregion

		#region Connection to controller
		
		public override void _Ready()
		{
			base._Ready();

			_ray = GetNode<RayCast2D>(__inFrontCheckPath);
		}

		public void Initialize(EnemyController controller)
		{
			Controller = controller;
		}

		public void Run(EnemyFrameData data)
		{
			CurrentData = data;
			Run(out bool mayCallExecute);

			if (mayCallExecute)
			{
				Execute();
			}
		}

		protected virtual void Run(out bool mayCallExecute)
		{
			mayCallExecute = !IsStunned();

			if (IsStunned())
			{
				WhenStunned();
			}
		}

		protected abstract void Execute();

		

		#endregion

		#region Basic behaviours
		protected void RunCombo(Combo which)
		{
			throw new NotImplementedException();
		}

		protected virtual bool CanSeeWall() => _ray.IsColliding();

		#endregion

		#region Damage reactions

		public virtual DamageReaction DamageTakenReaction() => DamageReaction.One;


		private float _stunDuration = 0f;
		public virtual void OnStunned(float stunDuration, EnemyFrameData data)
		{
			_stunDuration = stunDuration;
			data.Velocity = Vector2.Zero;
		}

		protected virtual bool IsStunned() => _stunDuration > 0f;

		protected virtual void WhenStunned()
		{
			_stunDuration -= CurrentData.Delta;

			if (!IsStunned())
			{
				CurrentData.Velocity.x = 0;
			}
		}

		public virtual void OnKnockedback(Vector2 velocity, EnemyFrameData data)
		{
			data.Velocity = velocity;
		}

		#endregion


	}
}