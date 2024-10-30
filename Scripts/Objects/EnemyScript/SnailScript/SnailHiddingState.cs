using da.Objects;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailHiddingState : EnemyState
    {
        private Snail _owner;
        public SnailHiddingState()
        {
            State = "hidding";
        }

        public override void Enter(Enemy owner)
        {
            _owner = owner as Snail;
            owner.Velocity = new(0, owner.Velocity.Y);
            owner.AnimPlayer.Play("hidding");
            _owner.IdleTimer.Timeout += ChangeToAppear;
            _owner.IdleTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            float y = owner.Velocity.Y + owner.Gravity * (float)delta;
            owner.Velocity = new(0, y);
        }

        private void ChangeToAppear()
        {
            _owner.StateMachine.ChangeState("appear");
        }

        public override void Exit(Enemy owner)
        {
            _owner.IdleTimer.Timeout -= ChangeToAppear;
        }
    }
}
