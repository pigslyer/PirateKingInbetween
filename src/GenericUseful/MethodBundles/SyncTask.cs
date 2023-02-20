using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// A kinda low level implementation of a simple Task pool, where each task contains an action it performs and a function that must return false until that function is done.
/// </summary>
public class SyncTaskPool
{

	private LinkedList<SyncTask> _tasks = new LinkedList<SyncTask>();

	public bool IsDone() => _tasks.Count == 0;

	public void Run()
	{
		LinkedListNode<SyncTask> node = _tasks.First;
		LinkedListNode<SyncTask> next;

		while (node != null)
		{
			node.Value.WhatDo();

			next = node.Next;

			if (node.Value.IsDone())
			{
				if (node.Value.OnDone != null)
				{
					next = _tasks.AddAfter(node, node.Value.OnDone);
					next.Value.WhatDo();
				}

				_tasks.Remove(node);
			}

			node = next;
		}
	}

	public IChainable<(Action, Func<bool>)> Register(Action whatDo, Func<bool> isDone)
	{
		_tasks.AddLast(new SyncTask((whatDo, isDone)));
		return _tasks.Last();
	}

	public void Clear()
	{
		_tasks.Clear();
	}

	class SyncTask : IChainable<(Action, Func<bool>)>
	{
		public Action WhatDo;
		public Func<bool> IsDone;
		public SyncTask OnDone;

		public SyncTask((Action whatDo, Func<bool> isDone) data)
		{
			WhatDo = data.whatDo; IsDone = data.isDone;
		}

		public IChainable<(Action, Func<bool>)> Chain((Action, Func<bool>) data)
		{
			return OnDone = new SyncTask(data);
		}
	}
}