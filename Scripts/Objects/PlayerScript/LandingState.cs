using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class LandingState : BaseState
    {
        public LandingState()
        {
            State = PlayerState.Landing;
        }

        private Player player;
        public override void Enter(Player owner)
        {
            player = owner;
            owner.CharactorAnimPlayer.Play("landing");
            owner.CharactorAnimPlayer.AnimationFinished += ChangeToIdle;
            // owner.ResetDashCount();
            owner.ResetJumpCount();
            owner.IsQuickDowned = false;
            owner.HasWallJumped = false;
            owner.CoyoteTimer.Stop();
            SoundManager.Ins.PlaySFX("Land");
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (direction.X != 0)
            {
                owner.StateMachine.ChangeState(PlayerState.Walk);
            }
            else if (!owner.JumpRequestTimer.IsStopped() && owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Jump);
            }
        }

        public override void Exit(Player owner)
        {
            owner.CharactorAnimPlayer.AnimationFinished -= ChangeToIdle;
        }

        private void ChangeToIdle(StringName animName)
        {
            player.StateMachine.ChangeState(PlayerState.Idle);
        }
    }
}
