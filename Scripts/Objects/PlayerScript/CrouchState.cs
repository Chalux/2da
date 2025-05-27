using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class CrouchState : BaseState
    {
        public CrouchState()
        {
            State = PlayerState.Crouch;
        }

        public override void Enter(Player player)
        {
            player.CharactorAnimPlayer.Play("squat");
            player.Collision.Shape = new RectangleShape2D() { Size = new(20, 20) };
            player.Collision.Position = new Vector2(0, -10);
            // player.ResetDashCount();
            player.ResetJumpCount();
            player.IsQuickDowned = false;
            player.HasWallJumped = false;
            player.CoyoteTimer.Stop();
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            owner.Velocity = new(0, 0);
            if (!owner.JumpRequestTimer.IsStopped())
            {
                owner.StateMachine.ChangeState(PlayerState.Jump);
            }
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (direction.Y <= 0.5)
            {
                owner.StateMachine.ChangeState(PlayerState.Idle);
            }
        }

        public override void UnhandledInput(InputEvent @event, Player owner)
        {
            if (!owner.AttackRequestTimer.IsStopped())
            {
                owner.StateMachine.ChangeState(PlayerState.Attack1);
            }
        }

        public override void Exit(Player owner)
        {
            owner.Collision.SetDeferred("shape", new RectangleShape2D() { Size = new(20, 27) });
            owner.Collision.SetDeferred("position", new Vector2(0, -13.5f));
        }
    }
}
