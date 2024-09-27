using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.PlayerScript
{
    internal class HurtState : BaseState
    {
        public HurtState()
        {
            State = PlayerState.Hurt;
        }
        private Player _owner;
        public override void Enter(Player owner)
        {
            _owner = owner;
            owner.CharactorAnimPlayer.Play("hurt");
            owner.CharactorAnimPlayer.AnimationFinished += ChangeToIdle;
            owner.HurtBox.SetDeferred("monitorable", false);
            owner.Velocity = new(100, -200);
            owner.MoveAndSlide();
        }

        public override void Exit(Player owner)
        {
            owner.CharactorAnimPlayer.AnimationFinished -= ChangeToIdle;
            owner.HurtBox.SetDeferred("monitorable", true);
        }

        private void ChangeToIdle(StringName animName)
        {
            if (_owner.IsOnFloor())
                _owner.StateMachine.ChangeState(PlayerState.Idle);
            else
            {
                _owner.StateMachine.ChangeState(PlayerState.Fall);
            }
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            float x = (float)Mathf.MoveToward(owner.Velocity.X, 0, delta);
            float y = owner.Velocity.Y + owner.gravity * (float)delta;
            owner.Velocity = new(x, y);
            if (owner.CanHurtMove)
            {
                Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
                if (direction.X != 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Walk);
                }
                else if (!owner.JumpRequestTimer.IsStopped() && owner.IsOnFloor())
                {
                    owner.StateMachine.ChangeState(PlayerState.Jump);
                }
            }
        }
    }
}
