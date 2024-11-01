using da.Objects;
using Godot;
using System;

namespace da.Scripts.Objects.PlayerScript
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
            owner.HasWallJumped = false;
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
                    if (Player.Speed / 2 > Math.Abs(owner.Velocity.X))
                    {
                        owner.StateMachine.ChangeState(PlayerState.Crouch);
                    }
                    else
                    {
                        owner.StateMachine.ChangeState(PlayerState.Slide);
                    }
                }
                else if (!owner.JumpRequestTimer.IsStopped())
                {
                    owner.StateMachine.ChangeState(PlayerState.Jump);
                }
                else if (direction.X == 0 && owner.Velocity.X == 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Idle);
                }
                //else if (Input.IsActionJustPressed("attack"))
                //{
                //    owner.StateMachine.ChangeState(PlayerState.Attack1);
                //}
            }
            else if (direction.Y < -0.5 && owner.LadderRay.IsColliding())
            {
                owner.StateMachine.ChangeState(PlayerState.ClimbLadder);
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

        public override void UnhandledInput(InputEvent @event, Player owner)
        {
            if ((owner.IsOnFloor() || !owner.CoyoteTimer.IsStopped()) && Input.IsActionJustPressed("attack"))
            {
                owner.StateMachine.ChangeState(PlayerState.Attack1);
            }
        }
    }
}
