using Godot;

namespace PirateInBetween.Game.Enemies
{
	public class TestEnemy : Node2D
	{
		[Export] private int _health = 3;

		public void OnDamage(DamageData data)
		{
			_health -= data.Damage.Amount;

			if (_health <= 0)
			{
				GD.Print("died");
				QueueFree();
			}
			else
			{
				GD.Print($"hit, remaining health: {_health}");
			}
		}
	}

}