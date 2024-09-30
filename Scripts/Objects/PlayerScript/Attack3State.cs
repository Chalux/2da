using da.Objects;
using Godot;
using System;

namespace da.Scripts.Objects.PlayerScript
{
    internal class Attack3State : BaseState
    {
        public Attack3State()
        {
            State = PlayerState.Attack3;
            CancelLevel = 3;
        }
        private Player _onwer;

        public override void Enter(Player player)
        {
            _onwer = player;
            player.CharactorAnimPlayer.Play("attack_3");
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
        }

        private void ChangeToIdle(StringName animName)
        {
            _onwer.StateMachine.ChangeState(PlayerState.Idle);
        }

        public override void Exit(Player owner)
        {
            _onwer.CharactorAnimPlayer.AnimationFinished -= ChangeToIdle;
            owner.CharactorAnimPlayer.CallDeferred("stop");
            owner.AttackCollision.Polygon = Array.Empty<Vector2>();
            owner.NextAttackState = null;
        }
    }
}
