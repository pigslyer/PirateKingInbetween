using Godot;
using System;
using System.Threading.Tasks;

namespace PirateInBetween.Game.Player
{
	public class PlayerModel : Node2D
	{

#region Paths

		[Export] private NodePath _shootFromPath = null;
		[Export] private NodePath _shootTargetPath = null;
		[Export] private NodePath _slasherPath = null;

#endregion

		private Position2D _shootFrom;
		private Position2D _shootTarget;
		private Slasher _slasher;

		public override void _Ready()
		{
			_shootFrom = GetNode<Position2D>(_shootFromPath);
			_shootTarget = GetNode<Position2D>(_shootTargetPath);
			_slasher = GetNode<Slasher>(_slasherPath);
		}

		public void SetAnimation(PlayerAnimation state, bool facingRight)
		{
			Scale = new Vector2(facingRight ? 1f : -1f, 1f);
		}
		

		public Task PlaySlash(SlashData data) => _slasher.Slash(data);

		public void Shoot(ProjectileData data)
		{
			IProjectile bullet = data.BulletScene.Instance<IProjectile>();
			bullet.AimAt(_shootFrom.GlobalPosition, _shootTarget, data, PhysicsLayers.WorldHittable);
		}
	}
}

