using Godot;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeFallState : EnemyState
    {
        public SlimeFallState()
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
