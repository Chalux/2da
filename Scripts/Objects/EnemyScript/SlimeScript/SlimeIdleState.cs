using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeIdleState : EnemyState
    {
        public SlimeIdleState()
        {
            State = "idle";
        }

        public override void OnAdd(Enemy owner)
        {
            (owner as Slime).IdleTimer.Timeout += () => ChangeToMove(owner as Slime);
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("idle");
            (owner as Slime).IdleTimer.Start();
            owner.Velocity = Vector2.Zero;
        }

        public override void AfterMove(double delta, Enemy owner)
        {
            Slime slime = (owner as Slime);
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState("fall");
            }
            if (slime.PlayerRay.IsColliding() && slime.FloorRay.IsColliding())
            {
                if (slime.PlayerRay.GetCollider() is Player player)
                {
                    slime.Target = player;
                    owner.StateMachine.ChangeState("move_to_player");
                }
            }
        }

        public override void Exit(Enemy owner)
        {
            (owner as Slime).IdleTimer.Stop();
        }

        private static void ChangeToMove(Slime slime)
        {
            if (!slime.FloorRay.IsColliding())
            {
                slime.Direction *= -1;
            }
            slime.StateMachine.ChangeState("move");
        }
    }
}
