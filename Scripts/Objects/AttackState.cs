using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.TextServer;

namespace da.Scripts.Objects
{
    internal class AttackState : BaseState
    {
        public AttackState()
        {
            State = PlayerState.Attack;
        }
        private Player _onwer;

        public override void Enter(Player player)
        {
            _onwer = player;
            player.CharactorAnimPlayer.Play("attack");
            player.Velocity = Vector2.Zero;
            player.CharactorAnimPlayer.AnimationFinished += ChangeToIdle;
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
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
            owner.CharactorAnimPlayer.Stop();
            owner.AttackCollision.Polygon = Array.Empty<Vector2>();
        }
    }
}
