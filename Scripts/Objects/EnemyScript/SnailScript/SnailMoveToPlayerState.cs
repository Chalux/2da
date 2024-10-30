using da.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailMoveToPlayerState : EnemyState
    {
        public SnailMoveToPlayerState()
        {
            State = "move_to_player";
        }

        public override void Enter(Enemy owner)
        {
            Snail Snail = owner as Snail;
            if (!Snail.FloorRay.IsColliding())
            {
                Snail.Direction *= -1;
            }
            Snail.AnimPlayer.Play("walk");
            Snail.CalmDownTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            Snail Snail = owner as Snail;
            if (!Snail.FloorRay.IsColliding())
            {
                owner.StateMachine.ChangeState("idle");
            }
            if (Snail.Target != null)
            {
                if (Snail.Position.X < Snail.Target.Position.X)
                {
                    Snail.Direction = 1;
                }
                else
                {
                    Snail.Direction = -1;
                }
            }
            EnemyStaticFunc.Move(Snail, Snail.Speed * 10, Snail.Acceleration * 20, (float)delta, 0f);
            if (Snail.PlayerRay.IsColliding())
            {
                if (Snail.PlayerRay.GetCollider() is Player player)
                {
                    Snail.Target = player;
                    Snail.CalmDownTimer.Start();
                }
            }
            if (Snail.Target != null && Math.Abs(Snail.Target.Position.X - Snail.Position.X) < Snail.AttackRange)
            {
                Snail.StateMachine.ChangeState("attack");
            }
            if (Snail.CalmDownTimer.IsStopped())
            {
                Snail.StateMachine.ChangeState("idle");
            }
        }

        public override void Exit(Enemy owner)
        {
            (owner as Snail).Target = null;
        }
    }
}
