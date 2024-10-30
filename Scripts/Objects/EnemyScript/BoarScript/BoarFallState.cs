using Godot;

namespace da.Scripts.Objects.EnemyScript.BoarScript
{
    internal class BoarFallState : EnemyState
    {
        public BoarFallState()
        {
            State = "fall";
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            EnemyStaticFunc.Move(owner, 0, 0, (float)delta, ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle());
            if (owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState("idle");
            }
        }
    }
}
