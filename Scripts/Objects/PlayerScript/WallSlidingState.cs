using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
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
            PlayerStaticFunc.Move(owner, delta, owner.gravity / 3);
        }

        public override void AfterMove(double delta, Player owner)
        {
            if (owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Idle);
            }
            else if (!owner.HandRay.IsColliding() || !owner.FootRay.IsColliding())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            else if (!owner.JumpRequestTimer.IsStopped())
            {
                owner.StateMachine.ChangeState(PlayerState.WallJump);
            }
        }
    }
}
