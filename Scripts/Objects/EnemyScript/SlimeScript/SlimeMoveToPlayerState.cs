using da.Objects;
using System;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeMoveToPlayerState : EnemyState
    {
        public SlimeMoveToPlayerState()
        {
            State = "move_to_player";
        }

        public override void Enter(Enemy owner)
        {
            Slime slime = owner as Slime;
            if (!slime.FloorRay.IsColliding())
            {
                slime.Direction *= -1;
            }
            slime.AnimPlayer.Play("move");
            slime.CalmDownTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            Slime slime = owner as Slime;
            if (!slime.FloorRay.IsColliding())
            {
                owner.StateMachine.ChangeState("idle");
            }
            if (slime.Target != null)
            {
                if (slime.Position.X < slime.Target.Position.X)
                {
                    slime.Direction = 1;
                }
                else
                {
                    slime.Direction = -1;
                }
            }
            EnemyStaticFunc.Move(slime, slime.Speed * 10, slime.Acceleration * 20, (float)delta, 0f);
            if (slime.PlayerRay.IsColliding())
            {
                if (slime.PlayerRay.GetCollider() is Player player)
                {
                    slime.Target = player;
                    slime.CalmDownTimer.Start();
                }
            }
            if (slime.Target != null && Math.Abs(slime.Target.Position.X - slime.Position.X) < slime.AttackRange)
            {
                slime.StateMachine.ChangeState("attack");
            }
            if (slime.CalmDownTimer.IsStopped())
            {
                slime.StateMachine.ChangeState("idle");
            }
        }

        public override void Exit(Enemy owner)
        {
            (owner as Slime).Target = null;
        }
    }
}
