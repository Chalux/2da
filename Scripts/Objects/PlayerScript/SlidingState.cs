using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class SlidingState : BaseState
    {
        private Player _owner;
        public SlidingState()
        {
            State = PlayerState.Sliding;
        }

        public override void Enter(Player owner)
        {
            _owner = owner;
            owner.CharactorAnimPlayer.Play("sliding");
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            float x = Mathf.MoveToward(owner.Velocity.X, 0, Player.FloorAcceleration / 5 * (float)delta);
            float y = owner.Velocity.Y;
            owner.Velocity = new Vector2(x, y);
        }

        public override void AfterMove(double delta, Player owner)
        {
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            if (owner.Velocity.X == 0)
            {
                owner.StateMachine.ChangeState(PlayerState.Idle);
            }
        }
    }
}
