using da.Objects;
using Godot;

namespace da.Scripts.Objects
{
    public class IdleState : BaseState
    {
        public IdleState()
        {
            State = PlayerState.Idle;
        }
        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("idle");
            owner.Velocity = Vector2.Zero;
            owner.ResetDashCount();
            owner.ResetJumpCount();
            owner.IsQuickDowned = false;
            owner.CoyoteTimer.Stop();
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            Vector2 velocity = owner.Velocity;
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
                return;
            }
            velocity.X = Mathf.MoveToward(owner.Velocity.X, 0, Player.Speed);
            owner.Velocity = velocity;
            // 状态转换
            if (direction.X != 0)
            {
                owner.StateMachine.ChangeState(PlayerState.Walk);
            }
            else if (!owner.JumpRequestTimer.IsStopped() && owner.IsOnFloor())
            {
                owner.TryJump();
            }
            else if (Input.IsActionPressed("attack"))
            {
                owner.StateMachine.ChangeState(PlayerState.Attack);
            }
            else if (owner.CheckCanDash && Input.IsActionPressed("dash"))
            {
                owner.StateMachine.ChangeState(PlayerState.Dash);
            }
            else if (direction.Y > 0.5)
            {
                owner.StateMachine.ChangeState(PlayerState.Crouch);
            }
        }
    }
}
