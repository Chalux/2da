using da.Objects;
using Godot;

namespace da.Scripts.Objects.EnemyScript.BoarScript
{
    internal class BoarIdleState : EnemyState
    {
        public BoarIdleState()
        {
            State = "idle";
        }

        public override void OnAdd(Enemy owner)
        {
            (owner as Boar).IdleTimer.Timeout += () => ChangeToMove(owner as Boar);
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("idle");
            (owner as Boar).IdleTimer.Start();
            owner.Velocity = Vector2.Zero;
        }

        public override void AfterMove(double delta, Enemy owner)
        {
            Boar Boar = (owner as Boar);
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState("fall");
            }
            if (Boar.PlayerRay.IsColliding() && Boar.FloorRay.IsColliding())
            {
                if (Boar.PlayerRay.GetCollider() is Player player)
                {
                    Boar.Target = player;
                    owner.StateMachine.ChangeState("ram");
                }
            }
        }

        public override void Exit(Enemy owner)
        {
            (owner as Boar).IdleTimer.Stop();
        }

        private static void ChangeToMove(Boar Boar)
        {
            if (!Boar.FloorRay.IsColliding())
            {
                Boar.Direction *= -1;
            }
            Boar.StateMachine.ChangeState("move");
        }
    }
}
