using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects.EnemyScript;
using da.Scripts.Objects.EnemyScript.BeeScript;
using Godot;

namespace da.Objects
{
    public partial class Bee : Enemy, IAttackable, IHurtable
    {
        [Export] public Timer IdleTimer;
        [Export] public Timer CalmDownTimer;
        public Player Target;
        public int AngryCount = 0;
        [Export] public Area2D ActionArea;
        [Export] public CollisionShape2D ActionShape;
        [Export] public Area2D AttackArea;

        public override void _Ready()
        {
            base._Ready();
            StateMachine.AddState("idle", new BeeIdleState());
            StateMachine.AddState("attack", new BeeAttackState());
            StateMachine.AddState("hurt", new BeeHurtState());
            StateMachine.AddState("death", new EnemyDeathState());
            StateMachine.ChangeState("idle");

            status.MaxHealth = 3;
            status.Health = status.MaxHealth;
            mirror = -1;
            Direction = -1;
            HurtBox.onHurt += BeeHurt;
            CalmDownTimer.Timeout += OnCalmDownTimerTimeout;
            Gravity = 0;

            TreeExited += OnTreeExited;
        }

        private void OnTreeExited()
        {
            ActionArea?.QueueFree();
        }

        private void BeeHurt(Damage damage)
        {
            status.Health -= damage.value;
            Direction = (damage.source.Owner as Player).Position.X > Position.X ? 1 : -1;
            if (status.Health <= 0)
            {
                SetCollisionMaskValue(1, true);
                StateMachine.ChangeState("death");
            }
            else
            {
                StateMachine.ChangeState("hurt");
            }
        }

        private void OnCalmDownTimerTimeout()
        {
            StateMachine.ChangeState("idle");
        }
    }
}
