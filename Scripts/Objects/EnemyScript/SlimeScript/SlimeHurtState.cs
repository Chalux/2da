using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeHurtState : EnemyState
    {
        public SlimeHurtState()
        {
            State = "hurt";
        }
        private Enemy _owner;
        public override void Enter(Enemy owner)
        {
            _owner = owner;
            owner.Velocity = new(-100 * owner.Direction, -200);
            owner.HitBox.SetDeferred("monitoring", false);
            owner.HurtBox.SetDeferred("monitorable", false);
            owner.AnimPlayer.Play("hurt");
            owner.AnimPlayer.AnimationFinished += ChangeToIdle;
        }

        private void ChangeToIdle(StringName animName)
        {
            _owner.StateMachine.ChangeState("idle");
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            float x = (float)Mathf.MoveToward(owner.Velocity.X, 0, delta);
            float y = owner.Velocity.Y + owner.Gravity * (float)delta;
            owner.Velocity = new(x, y);
        }

        public override void Exit(Enemy owner)
        {
            owner.AnimPlayer.AnimationFinished -= ChangeToIdle;
            owner.HitBox.SetDeferred("monitoring", true);
            owner.HurtBox.SetDeferred("monitorable", true);
        }
    }
}
