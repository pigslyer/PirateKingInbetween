using Godot;
using System.Collections.Generic;

namespace PirateInBetween.Game
{
	public class MovingParentDetector : RayCast2D
	{
		[Signal] delegate void OnChangedParent();

		#region Paths

		[Export] private NodePath _movingWhoPath;

		#endregion

		private Node2D _movingWho;
		private MovingParent _currentMovingParent;
		private HashSet<Node> _checked = new HashSet<Node>();

		public override void _Ready()
		{
			_movingWho = GetNode<Node2D>(_movingWhoPath);
			_currentMovingParent = MovingParent.GetMovingParentOf(_movingWho);
		}

		public override void _PhysicsProcess(float _)
		{
			if (IsColliding() && GetCollider() is Node node && !_checked.Contains(node) && MovingParent.TryGetMovingParentOf(node, out var mp))
			{
				if (mp != null && mp != _currentMovingParent)
				{
					mp.Move(_movingWho);
					_currentMovingParent = mp;
					_checked.Clear();

					EmitSignal(nameof(OnChangedParent));
				}
				else
				{
					_checked.Add(node);
				}
			}
		}
	} 
}