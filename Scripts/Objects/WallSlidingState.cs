using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    internal class WallSlidingState : BaseState
    {
        public WallSlidingState()
        {
            State = PlayerState.WallSliding;
        }

        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("wall_sliding");
            owner.Velocity = Vector2.Zero;
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            var gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
            PlayerStaticFunc.Move(owner, delta, gravity / 3);
            if (owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Idle);
            }
            else if (!owner.IsOnWall())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            else if (!owner.JumpRequestTimer.IsStopped())
            {
                owner.TryJump();
            }
        }
    }
}
