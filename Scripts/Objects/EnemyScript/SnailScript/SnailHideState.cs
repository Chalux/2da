using da.Objects;
using Godot;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailHideState : EnemyState
    {
        Snail _owner;
        public SnailHideState()
        {
            State = "hide";
        }

        public override void Enter(Enemy owner)
        {
            _owner = owner as Snail;
            owner.AnimPlayer.Play("hide");
            owner.AnimPlayer.AnimationFinished += ChangeToHidding;
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            float x = (float)Mathf.MoveToward(owner.Velocity.X, 0, delta);
            float y = owner.Velocity.Y + owner.Gravity * (float)delta;
            owner.Velocity = new(x, y);
        }

        private void ChangeToHidding(StringName animName)
        {
            if (animName == "hide")
                _owner.StateMachine.ChangeState("hidding");
        }

        public override void Exit(Enemy owner)
        {
            owner.AnimPlayer.AnimationFinished -= ChangeToHidding;
        }
    }
}
