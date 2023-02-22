using Godot;
using System.Collections.Generic;

namespace PirateInBetween.Game
{
	public class MovingParentDetector : RayCast2D
	{
		public event EventDelegates.OnSomethingHappened<MovingParent> OnMovingParentChanged;

		#region Paths

		[Export] private NodePath _movingWhoPath = null;

		#endregion

		private Node2D _movingWho;
		public MovingParent CurrentMovingParent { get; private set; } = null;
		private HashSet<Node> _checked = new HashSet<Node>();

		public override void _Ready()
		{
			_movingWho = GetNode<Node2D>(_movingWhoPath);
			CurrentMovingParent = MovingParent.GetMovingParentOf(_movingWho);
			OnMovingParentChanged?.Invoke(CurrentMovingParent);
		}

		public override void _PhysicsProcess(float _)
		{
			if (IsColliding() && GetCollider() is Node node && !_checked.Contains(node) && MovingParent.TryGetMovingParentOf(node, out var mp))
			{
				if (mp != null && mp != CurrentMovingParent)
				{
					SetMovingParent(mp);
				}
				
				_checked.Add(node);
			}
		}

		public void SetDetecting(bool state)
		{
			SetPhysicsProcess(state);
			
			if (!state)
			{
				CurrentMovingParent = null;
			}
		}

		public void SetMovingParent(MovingParent newParent)
		{
			newParent.Move(_movingWho);
			CurrentMovingParent = newParent;
			_checked.Clear();

			OnMovingParentChanged?.Invoke(newParent);
		}
	} 
}