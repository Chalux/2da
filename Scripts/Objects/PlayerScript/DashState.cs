using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class DashState : BaseState
    {
        public float oldVelocityY;
        private Vector2 Angle = Vector2.Zero;

        public DashState()
        {
            State = PlayerState.Dash;
        }

        public override void Enter(Player owner)
        {
            if (!owner.DashTimer.IsStopped() || !owner.TryDash())
            {
                owner.EndDash();
            }
            else
            {
                owner.Dash();
                if (owner.StateMachine.oldStateEnum == PlayerState.WallSliding)
                {
                    owner.CharactorAnimPlayer.Play("fall");
                    owner.Direction = owner.GetWallNormal().X > 0 ? 1 : -1;
                    Angle = new Vector2(owner.Direction, 0);
                }
                else
                {
                    if (Input.IsActionPressed("ui_left"))
                    {
                        owner.Direction = -1;
                    }
                    else if (Input.IsActionPressed("ui_right"))
                    {
                        owner.Direction = 1;
                    }

                    Angle = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
                    if (Angle == Vector2.Zero)
                    {
                        Angle = new Vector2(owner.Direction, 0);
                    }
                }

                //oldVelocityY = owner.Velocity.Y;
                oldVelocityY = 0;
                owner.JumpRequestTimer.Stop();
                SoundManager.Ins.PlaySFX("Dash");
            }
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            //owner.Velocity = new(owner.DashSpeed * owner.Direction, 0);
            owner.Velocity = new(owner.DashSpeed * Angle.X, owner.DashSpeed * Angle.Y);
            if (Input.IsActionPressed("ui_down") && !owner.IsQuickDowned && !owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.QuickDown);
            }
        }

        public override void Exit(Player owner)
        {
            owner.Velocity = new(owner.Velocity.X, oldVelocityY);
        }
    }
}