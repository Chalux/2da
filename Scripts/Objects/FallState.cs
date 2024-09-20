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
    internal class FallState : BaseState
    {
        public FallState()
        {
            State = PlayerState.Fall;
        }

        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("fall");
            owner.CoyoteTimer.Stop();
            owner.HasReleasedCrouchKey = !Input.IsActionPressed("ui_down");
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

            PlayerStaticFunc.Move(owner, delta, ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle());
            if (owner.IsOnFloor())
            {
                if (direction.Y > 0.5)
                {
                    owner.StateMachine.ChangeState(PlayerState.Crouch);
                }
                else if (direction.X == 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Landing);
                }
                else
                {
                    owner.StateMachine.ChangeState(PlayerState.Walk);
                }
            }
            else if (Input.IsActionPressed("ui_down") && !owner.IsQuickDowned && owner.HasReleasedCrouchKey)
            {
                owner.StateMachine.ChangeState(PlayerState.QuickDown);
            }
            else if (Input.IsActionPressed("ui_accept") && owner.HasReleasedJumpKey)
            {
                owner.TryJump();
            }
            else if (owner.IsOnWall() && owner.HandRay.IsColliding() && owner.FootRay.IsColliding())
            {
                owner.StateMachine.ChangeState(PlayerState.WallSliding);
            }
        }
    }
}
