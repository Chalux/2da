using da.Objects;
using Godot;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailAppearState : EnemyState
    {
        private Snail _owner;
        public SnailAppearState()
        {
            State = "appear";
        }

        public override void Enter(Enemy owner)
        {
            _owner = owner as Snail;
            owner.AnimPlayer.Play("appear");
            owner.AnimPlayer.AnimationFinished += ChangeToIdle;
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            float y = owner.Velocity.Y + owner.Gravity * (float)delta;
            owner.Velocity = new(0, y);
        }

        private void ChangeToIdle(StringName animName)
        {
            if (animName == "appear")
                _owner.StateMachine.ChangeState("idle");
        }

        public override void Exit(Enemy owner)
        {
            owner.AnimPlayer.AnimationFinished -= ChangeToIdle;
        }
    }
}
