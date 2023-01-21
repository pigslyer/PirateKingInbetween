using System.Runtime.InteropServices;
using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Player
{
    public class PlayerBehaviourManager : Node
    {		

        /// <summary>
        /// Dictates how long it takes for <see cref="PlayerBehaviour"/> to realise that the player isn't on the floor.
        /// </summary>
        [Export] private float _playerCoyoteTimeLength = 0.3f;

        private PlayerController _player;

        private List<PlayerBehaviour> _behaviours = new List<PlayerBehaviour>();
        public PlayerBehaviour.Behaviours ActiveBehaviours = PlayerBehaviour.Behaviours.Default;
		private PlayerBehaviour _activeStoppingBehaviour = null;
        
        public bool CanChangeActive(PlayerBehaviour who) => _activeStoppingBehaviour == null || _activeStoppingBehaviour == who;
        public void SetStoppingBehaviourActive(PlayerBehaviour stopper)
        {
            _activeStoppingBehaviour = stopper;
        }

		public bool IsBehaviourActive(PlayerBehaviour.Behaviours behaviour) => (ActiveBehaviours & behaviour) != 0;

        public void Initialize(PlayerController player)
        {
            _player = player;

            foreach (var child in GetChildren())
            {
                if (child is PlayerBehaviour beh)
                {
                    beh.Initialize(player, this);
                    _behaviours.Add(beh);
                }
                else
                {
                    GD.PushError($"Why is a direct descendant of {nameof(PlayerBehaviourManager)} not of type {nameof(PlayerBehaviour)}?");
                }
            }
        }

        /// <summary>
        /// Runs all behaviours associated with this <see cref="PlayerBehaviourManager"/>, passing them
        /// the given <see cref="PlayerCurrentFrameData"/> to be edited.
        /// </summary>
        public void RunBehaviours(PlayerCurrentFrameData data)
        {			
            for (int i = 0; i < _behaviours.Count; i++)
			{
				if (IsBehaviourActive((PlayerBehaviour.Behaviours)(1 << i)))
				{
					_behaviours[i].Run(data);
				}
			}
        }

        private float _timeSinceOnFloor = 0f;
        public override void _PhysicsProcess(float delta)
        {
            if (_player.IsOnFloor())
            {
                _timeSinceOnFloor = 0f;
            }
            else
            {
                _timeSinceOnFloor += delta;
            }
        }

        public bool IsPlayerOnFloor() => _timeSinceOnFloor < _playerCoyoteTimeLength;
        
        private PlayerBehaviour.Behaviours _previous = PlayerBehaviour.Behaviours.Default;

		public override void _Input(InputEvent @event)
		{
			if (OS.IsDebugBuild() && @event.IsActionPressed("temp_noclip"))
			{
				if (IsBehaviourActive(PlayerBehaviour.Behaviours.Noclip))
				{
					ActiveBehaviours = _previous;
				}
				else
				{
					_previous = ActiveBehaviours;
					ActiveBehaviours = PlayerBehaviour.Behaviours.Noclip;
				}
			}
		}
    }
}