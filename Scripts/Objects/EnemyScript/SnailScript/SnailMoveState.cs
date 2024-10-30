using da.Objects;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailMoveState : EnemyState
    {
        public SnailMoveState()
        {
            State = "move";
        }

        public override void OnAdd(Enemy owner)
        {
            Snail Snail = owner as Snail;
            Snail.AnimPlayer.Play("walk");
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("walk");
            (owner as Snail).CalmDownTimer.Start();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            Snail Snail = owner as Snail;

            EnemyStaticFunc.Move(Snail, Snail.Speed, Snail.Acceleration, (float)delta, 0f);
        }

        public override void AfterMove(double delta, Enemy owner)
        {
            Snail Snail = owner as Snail;
            if (!Snail.FloorRay.IsColliding())
            {
                owner.StateMachine.ChangeState("idle");
            }
            if (Snail.WallRay.IsColliding())
            {
                owner.Direction *= -1;
                owner.StateMachine.ChangeState("idle");
            }
            if (Snail.PlayerRay.IsColliding())
            {
                if (Snail.PlayerRay.GetCollider() is Player player)
                {
                    Snail.Target = player;
                    owner.StateMachine.ChangeState("move_to_player");
                }
            }
        }
    }
}
