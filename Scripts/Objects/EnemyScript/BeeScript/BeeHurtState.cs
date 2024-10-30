using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript.BeeScript
{
    public class BeeHurtState : EnemyState
    {
        public BeeHurtState()
        {
            State = "hurt";
        }
        private Bee _owner;
        public override void Enter(Enemy owner)
        {
            _owner = owner as Bee;
            owner.Velocity = new(-100 * owner.Direction, 0);
            owner.HitBox.SetDeferred("monitoring", false);
            owner.HurtBox.SetDeferred("monitorable", false);
            owner.AnimPlayer.Play("hurt");
            owner.AnimPlayer.AnimationFinished += ChangeToIdle;
        }

        private void ChangeToIdle(StringName animName)
        {
            if (_owner.AngryCount >= 3)
            {
                _owner.StateMachine.ChangeState("attack");
            }
            else
            {
                _owner.StateMachine.ChangeState("idle");
            }
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
