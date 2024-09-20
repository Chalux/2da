using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.TextServer;

namespace da.Scripts.Objects
{
    internal class WalkState : BaseState
    {
        public WalkState()
        {
            State = PlayerState.Walk;
        }
        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("running");
            owner.ResetDashCount();
            owner.ResetJumpCount();
            owner.IsQuickDowned = false;
            owner.CoyoteTimer.Stop();
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            PlayerStaticFunc.Move(owner, delta, 0);
        }

        public override void AfterMove(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (owner.IsOnFloor() || !owner.CoyoteTimer.IsStopped())
            {
                if (direction.Y > 0.5)
                {
                    owner.StateMachine.ChangeState(PlayerState.Crouch);
                }
                else if (!owner.JumpRequestTimer.IsStopped())
                {
                    owner.StateMachine.ChangeState(PlayerState.Jump);
                }
                else if (direction.X == 0 && owner.Velocity.X == 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Idle);
                }
                else if (Input.IsActionJustPressed("attack"))
                {
                    owner.StateMachine.ChangeState(PlayerState.Attack);
                }
            }
            else
            {
                if (direction.Y > 0.5)
                {
                    owner.StateMachine.ChangeState(PlayerState.QuickDown);
                }
                else if (owner.CoyoteTimer.IsStopped())
                {
                    owner.CoyoteTimer.Start();
                }
            }
        }
    }
}
