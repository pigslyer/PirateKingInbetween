using Godot;

namespace PirateInBetween.Game.Enemies
{
	public class TestEnemy : Node2D
	{
		#region Paths
		[Export] private NodePath __damageTakerPath = null;
		#endregion

		[Export] private int _health = 3;

		public override void _Ready()
		{
			base._Ready();

			GetNode<DamageTaker>(__damageTakerPath).OnDamageTaken += OnDamage;
		}

		public void OnDamage(DamageTaker takerSource, DamageDealer dealerSource, DamageData data)
		{
			_health -= data.Damage;

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