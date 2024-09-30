using da.Objects;
using Godot;
using System;

namespace da.Scripts.Objects.PlayerScript
{
    internal class Attack2State : BaseState
    {
        public Attack2State()
        {
            State = PlayerState.Attack2;
            CancelLevel = 2;
        }
        private Player _onwer;

        public override void Enter(Player player)
        {
            _onwer = player;
            player.CharactorAnimPlayer.Play("attack_2");
            player.Velocity = Vector2.Zero;
            player.CharactorAnimPlayer.AnimationFinished += ChangeToIdle;
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (direction.X != 0)
            {
                player.Direction = direction.X > 0 ? 1 : -1;
            }
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            if (!owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            if (!owner.AttackRequestTimer.IsStopped() && owner.CanCancelAttack)
            {
                owner.NextAttackState = owner.StateMachine.stateDict[PlayerState.Attack3];
            }
        }

        private void ChangeToIdle(StringName animName)
        {
            if (_onwer.NextAttackState != null && _onwer.NextAttackState.CancelLevel > CancelLevel)
            {
                _onwer.StateMachine.ChangeState(_onwer.NextAttackState.State);
            }
            else
            {
                _onwer.StateMachine.ChangeState(PlayerState.Idle);
            }
            _onwer.NextAttackState = null;
        }

        public override void Exit(Player owner)
        {
            _onwer.CharactorAnimPlayer.AnimationFinished -= ChangeToIdle;
            owner.CharactorAnimPlayer.Stop();
            owner.AttackCollision.Polygon = Array.Empty<Vector2>();
        }
    }
}
