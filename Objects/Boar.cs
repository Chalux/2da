using da.Scripts;
using da.Scripts.Objects.EnemyScript;
using da.Scripts.Objects.EnemyScript.BoarScript;
using Godot;

namespace da.Objects
{
    public partial class Boar : Enemy
    {
        [Export] public RayCast2D FloorRay;
        [Export] public RayCast2D PlayerRay;
        [Export] public RayCast2D WallRay;
        [Export] public Timer IdleTimer;
        [Export] public Timer CalmDownTimer;
        public Player Target;

        public override void _Ready()
        {
            base._Ready();
            StateMachine.AddState("idle", new BoarIdleState());
            StateMachine.AddState("move", new BoarMoveState());
            StateMachine.AddState("ram", new BoarRamState());
            StateMachine.AddState("fall", new BoarFallState());
            StateMachine.AddState("hurt", new BoarHurtState());
            StateMachine.AddState("death", new EnemyDeathState());
            StateMachine.ChangeState("idle");

            mirror = -1;
            Direction = -1;
            HurtBox.onHurt += BoarHurt;
        }

        private void BoarHurt(Damage damage)
        {
            status.Health -= damage.value;
            Direction = (damage.source.Owner as Player)?.Position.X > Position.X ? 1 : -1;
            StateMachine.ChangeState(status.Health <= 0 ? "death" : "hurt");
        }
    }
}
