using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class SlideState : BaseState
    {
        private Player _owner;
        public SlideState()
        {
            State = PlayerState.Slide;
        }

        public override void Enter(Player owner)
        {
            _owner = owner;
            owner.CharactorAnimPlayer.Play("slide");
            owner.CharactorAnimPlayer.AnimationFinished += ChangeToSliding;
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            owner.Velocity = new(Mathf.MoveToward(owner.Velocity.X, 0, Player.FloorAcceleration / 5 * (float)delta), owner.Velocity.Y);
            if (owner.Velocity.X == 0)
            {
                owner.StateMachine.ChangeState(PlayerState.Idle);
            }
        }

        private void ChangeToSliding(StringName animName)
        {
            _owner.StateMachine.ChangeState(PlayerState.Sliding);
        }

        public override void Exit(Player owner)
        {
            owner.CharactorAnimPlayer.AnimationFinished -= ChangeToSliding;
        }
    }
}
