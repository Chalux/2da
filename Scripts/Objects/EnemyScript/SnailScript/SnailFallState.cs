namespace da.Scripts.Objects.EnemyScript.SnailScript
{
    internal class SnailFallState : EnemyState
    {
        public SnailFallState()
        {
            State = "fall";
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            EnemyStaticFunc.Move(owner, 0, 0, (float)delta, owner.Gravity);
            if (owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState("idle");
            }
        }
    }
}
