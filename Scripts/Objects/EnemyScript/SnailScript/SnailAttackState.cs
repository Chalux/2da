using da.Objects;
using Godot;
using System;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailAttackState : EnemyState
    {
        private Snail owner;
        public SnailAttackState()
        {
            State = "attack";
        }

        public override void OnAdd(Enemy owner)
        {
            this.owner = owner as Snail;
        }

        private void OnAnimationFinished(StringName animName)
        {
            if (animName == "attack")
            {
                Random r = new();
                if (r.Next(0, 100) < 50) // 50% chance to go to idle
                    owner.StateMachine.ChangeState("idle");
                else
                {
                    owner.StateMachine.ChangeState("hide");
                }
            }
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("attack");
            owner.AnimPlayer.AnimationFinished += OnAnimationFinished;
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
