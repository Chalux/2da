using da.Objects;
using Godot;

namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailIdleState : EnemyState
    {
        public SnailIdleState()
        {
            State = "idle";
        }

        public override void OnAdd(Enemy owner)
        {
            (owner as Snail).IdleTimer.Timeout += () => ChangeToMove(owner as Snail);
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("idle");
            (owner as Snail).IdleTimer.Start();
            owner.Velocity = Vector2.Zero;
        }

        public override void AfterMove(double delta, Enemy owner)
        {
            Snail Snail = (owner as Snail);
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState("fall");
            }
            if (Snail.PlayerRay.IsColliding() && Snail.FloorRay.IsColliding())
            {
                if (Snail.PlayerRay.GetCollider() is Player player)
                {
                    Snail.Target = player;
                    owner.StateMachine.ChangeState("move_to_player");
                }
            }
        }

        public override void Exit(Enemy owner)
        {
            (owner as Snail).IdleTimer.Stop();
        }

        private static void ChangeToMove(Snail Snail)
        {
            if (!Snail.FloorRay.IsColliding())
            {
                Snail.Direction *= -1;
            }
            Snail.StateMachine.ChangeState("move");
        }
    }
}
