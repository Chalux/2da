using da.Scripts;
using da.Scripts.Objects.EnemyScript;
using da.Scripts.Objects.EnemyScript.SnailScript;
using Godot;

namespace da.Objects
{
    public partial class Snail : Enemy
    {
        [Export] public RayCast2D FloorRay;
        [Export] public RayCast2D PlayerRay;
        [Export] public RayCast2D WallRay;
        [Export] public Timer IdleTimer;
        [Export] public Timer CalmDownTimer;
        public Player Target;
        [Export] public int AttackRange;

        public override void _Ready()
        {
            base._Ready();
            StateMachine.AddState("idle", new SnailIdleState());
            StateMachine.AddState("move", new SnailMoveState());
            StateMachine.AddState("attack", new SnailAttackState());
            StateMachine.AddState("move_to_player", new SnailMoveToPlayerState());
            StateMachine.AddState("fall", new SnailFallState());
            StateMachine.AddState("hurt", new SnailHurtState());
            StateMachine.AddState("death", new EnemyDeathState());
            StateMachine.AddState("hide", new SnailHideState());
            StateMachine.AddState("hidding", new SnailHiddingState());
            StateMachine.AddState("appear", new SnailAppearState());
            StateMachine.ChangeState("idle");

            mirror = -1;
            Direction = -1;
            HurtBox.onHurt += SnailHurt;
        }

        private void SnailHurt(Damage damage)
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
