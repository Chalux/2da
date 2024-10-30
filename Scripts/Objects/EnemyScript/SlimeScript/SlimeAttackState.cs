using da.Objects;
using Godot;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeAttackState : EnemyState
    {
        private Slime owner;
        public SlimeAttackState()
        {
            State = "attack";
        }

        public override void OnAdd(Enemy owner)
        {
            this.owner = owner as Slime;
            owner.AnimPlayer.AnimationFinished += OnAnimationFinished;
        }

        private void OnAnimationFinished(StringName animName)
        {
            if (animName == "attack")
            {
                owner.StateMachine.ChangeState("idle");
            }
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("attack");
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState("fall");
            }
        }

        public override void Exit(Enemy owner)
        {
            owner.AnimPlayer.AnimationFinished -= OnAnimationFinished;
        }
    }
}
