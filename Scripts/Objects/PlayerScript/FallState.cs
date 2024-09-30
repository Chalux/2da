using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
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
            PlayerStaticFunc.Move(owner, delta, owner.gravity);
        }

        public override void AfterMove(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
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
