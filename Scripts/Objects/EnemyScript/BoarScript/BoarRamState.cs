using da.Objects;

namespace da.Scripts.Objects.EnemyScript.BoarScript
{
    internal class BoarRamState : EnemyState
    {
        public BoarRamState()
        {
            State = "ram";
        }

        public override void Enter(Enemy owner)
        {
            Boar Boar = owner as Boar;
            if (!Boar.FloorRay.IsColliding())
            {
                Boar.Direction *= -1;
            }
            Boar.AnimPlayer.Play("move");
            Boar.CalmDownTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            Boar Boar = owner as Boar;
            if (!Boar.FloorRay.IsColliding())
            {
                owner.StateMachine.ChangeState("fall");
            }
            if (Boar.WallRay.IsColliding())
            {
                Boar.Direction *= -1;
            }
            EnemyStaticFunc.Move(Boar, Boar.Speed * 7, Boar.Acceleration * 10, (float)delta, 0f);
            if (Boar.CalmDownTimer.IsStopped())
            {
                Boar.StateMachine.ChangeState("idle");
            }
        }

        public override void Exit(Enemy owner)
        {
            (owner as Boar).Target = null;
        }
    }
}
