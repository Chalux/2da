using da.Scripts;
using da.Scripts.Objects.EnemyScript;
using da.Scripts.Objects.EnemyScript.SlimeScript;
using Godot;

namespace da.Objects
{
    internal partial class Slime : Enemy
    {
        [Export] public RayCast2D FloorRay;
        [Export] public RayCast2D PlayerRay;
        [Export] public RayCast2D WallRay;
        [Export] public Timer IdleTimer;
        [Export] public Timer CalmDownTimer;
        public float AttackRange = 15f;

        public Player Target;

        public override void _Ready()
        {
            base._Ready();
            StateMachine.AddState("idle", new SlimeIdleState());
            StateMachine.AddState("move", new SlimeMoveState());
            StateMachine.AddState("attack", new SlimeAttackState());
            StateMachine.AddState("move_to_player", new SlimeMoveToPlayerState());
            StateMachine.AddState("fall", new SlimeFallState());
            StateMachine.AddState("hurt", new SlimeHurtState());
            StateMachine.AddState("death", new EnemyDeathState());
            StateMachine.ChangeState("idle");

            mirror = -1;
            Direction = -1;
            HurtBox.onHurt += SlimeHurt;
        }

        private void SlimeHurt(Damage damage)
        {
            status.Health -= damage.value;
            Direction = (damage.source.Owner as Player).Position.X > Position.X ? 1 : -1;
            if (status.Health <= 0)
            {
                StateMachine.ChangeState("death");
            }
            else
            {
                StateMachine.ChangeState("hurt");
            }
        }
    }
}
