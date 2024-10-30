using da.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;

namespace da.Scripts.Objects.EnemyScript.BoarScript
{
    internal class BoarMoveState : EnemyState
    {
        public BoarMoveState()
        {
            State = "move";
        }

        public override void OnAdd(Enemy owner)
        {
            Boar Boar = owner as Boar;
            Boar.AnimPlayer.Play("move");
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("move");
            (owner as Boar).CalmDownTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            Boar Boar = owner as Boar;

            EnemyStaticFunc.Move(Boar, Boar.Speed, Boar.Acceleration, (float)delta, 0f);
        }

        public override void AfterMove(double delta, Enemy owner)
        {
            Boar Boar = owner as Boar;
            if (!Boar.FloorRay.IsColliding())
            {
                owner.StateMachine.ChangeState("idle");
            }
            if (Boar.WallRay.IsColliding())
            {
                owner.Direction *= -1;
                owner.StateMachine.ChangeState("idle");
            }
            if (Boar.PlayerRay.IsColliding())
            {
                if (Boar.PlayerRay.GetCollider() is Player player)
                {
                    Boar.Target = player;
                    owner.StateMachine.ChangeState("ram");
                }
            }
        }
    }
}
