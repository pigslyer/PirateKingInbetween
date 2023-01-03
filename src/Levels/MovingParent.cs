using Godot;

namespace PirateInBetween.Game
{
	/// <summary>
	/// Parent which automatically moves over time.
	/// </summary>
	public class MovingParent : Node2D
	{
		/// <summary>
		/// Changes what to be child of this MovingParent, removing previous parent and maintaining global position.
		/// </summary>
		public void Move(Node2D what)
		{
			Vector2 prevPos = what.GlobalPosition;
			
			what.GetParent().RemoveChild(what);
			AddChild(what);

			what.GlobalPosition = prevPos;
		}


		/// <summary>
		/// Wrapper for GetMovingParentOf which follows the try pattern.
		/// </summary>
		public static bool TryGetMovingParentOf(Node what, out MovingParent movingParent)
		{
			movingParent = GetMovingParentOf(what);
			return movingParent != null;
		}

		/// <summary>
		/// Attempts to find MovingParent of given node.
		/// </summary>
		public static MovingParent GetMovingParentOf(Node what)
		{
			Node current = what;

			while (current != null)
			{
				if (current is MovingParent p)
				{
					return p;
				}

				current = current.GetParent();
			}

			return null;
		}
	}
}