using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailHurtState : EnemyState
    {
        public SnailHurtState()
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
            if (animName == "hurt")
            {
                Random r = new();
                if (r.Next(0, 2) == 0)
                {
                    _owner.StateMachine.ChangeState("idle");
                }
                else
                {
                    _owner.StateMachine.ChangeState("hide");
                }
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
