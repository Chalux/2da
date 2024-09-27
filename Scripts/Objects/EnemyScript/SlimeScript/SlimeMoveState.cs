using da.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeMoveState : EnemyState
    {
        public SlimeMoveState()
        {
            State = "move";
        }

        public override void OnAdd(Enemy owner)
        {
            Slime slime = owner as Slime;
            slime.AnimPlayer.Play("move");
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("move");
            (owner as Slime).CalmDownTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            Slime slime = owner as Slime;

            EnemyStaticFunc.Move(slime, slime.Speed, slime.Acceleration, (float)delta, 0f);
        }

        public override void AfterMove(double delta, Enemy owner)
        {
            Slime slime = owner as Slime;
            if (!slime.FloorRay.IsColliding())
            {
                owner.StateMachine.ChangeState("idle");
            }
            if (slime.WallRay.IsColliding())
            {
                owner.Direction *= -1;
                owner.StateMachine.ChangeState("idle");
            }
            if (slime.PlayerRay.IsColliding())
            {
                if (slime.PlayerRay.GetCollider() is Player player)
                {
                    slime.Target = player;
                    owner.StateMachine.ChangeState("move_to_player");
                }
            }
        }
    }
}
