using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class JumpState : BaseState
    {
        public JumpState()
        {
            State = PlayerState.Jump;
        }
        private Player _owner;

        public override void Enter(Player owner)
        {
            _owner = owner;
            owner.CharactorAnimPlayer.Play("jump");
            owner.JumpCount -= 1;
            owner.Velocity = new Vector2(owner.Velocity.X, Player.JumpVelocity);
            owner.MoveAndSlide();
            owner.CharactorAnimPlayer.AnimationFinished += ChangeToFall;
            owner.JumpTimer.Start();
            owner.JumpRequestTimer.Stop();
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
                    SoundManager.Ins.PlaySFX("Land");
                    owner.StateMachine.ChangeState(PlayerState.Crouch);
                }
                else if (direction.X == 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Landing);
                }
                else
                {
                    SoundManager.Ins.PlaySFX("Land");
                    owner.StateMachine.ChangeState(PlayerState.Walk);
                }
            }
            else if (owner.IsOnWall() && owner.HandRay.IsColliding() && owner.FootRay.IsColliding())
            {
                owner.StateMachine.ChangeState(PlayerState.WallSliding);
            }
            else
            {
                if (direction.Y > 0.5 && !owner.IsQuickDowned && owner.HasReleasedCrouchKey)
                {
                    owner.StateMachine.ChangeState(PlayerState.QuickDown);
                }
                else if (!owner.JumpRequestTimer.IsStopped())
                {
                    if (owner.CheckCanJump && owner.HasReleasedJumpKey)
                    {
                        owner.CharactorAnimPlayer.AnimationFinished -= ChangeToFall;
                        Enter(owner);
                    }
                }
            }
        }

        public void ChangeToFall(StringName animName)
        {
            _owner.StateMachine.ChangeState(PlayerState.Fall);
        }

        public override void Exit(Player owner)
        {
            _owner.CharactorAnimPlayer.AnimationFinished -= ChangeToFall;
        }
    }
}
