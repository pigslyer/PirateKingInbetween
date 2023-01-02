using Godot;

namespace PirateInBetween.Game.Enemies
{
	public class TestEnemy : StaticBody2D, IHittable
	{
		[Export] private int _health = 3;

		public void Hit(HitData data)
		{
			if (_health > 0)
			{
				GD.Print("ow!");
				_health--;
			}
			else
			{
				GD.Print("Aaaaa!");
				QueueFree();
			}
		}
	}

}