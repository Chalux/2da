using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
