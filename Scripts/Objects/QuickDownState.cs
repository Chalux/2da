using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    internal class QuickDownState : BaseState
    {
        private SceneTreeTimer timer;
        public QuickDownState()
        {
            State = PlayerState.QuickDown;
        }

        public override void Enter(Player owner)
        {
            owner.IsQuickDowned = true;
            owner.GhostTimer.Start();
            owner.DashParticles.Emitting = true;
            timer = owner.GetTree().CreateTimer(0.25f);
            timer.Timeout += () =>
            {
                CheckChangeState(owner);
            };
            owner.JumpRequestTimer.Stop();
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            owner.Velocity = new(0, owner.DashSpeed);
            CheckChangeState(owner);
        }

        private void CheckChangeState(Player owner)
        {
            if (owner.IsOnFloor())
            {
                var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
                if (direction.Y > 0.5)
                {
                    owner.StateMachine.ChangeState(PlayerState.Crouch);
                }
                else if (direction != Vector2.Zero)
                {
                    owner.StateMachine.ChangeState(PlayerState.Walk);
                }
                else
                {
                    owner.StateMachine.ChangeState(PlayerState.Idle);
                }
            }
            else
            {
                if (timer == null || timer.TimeLeft == 0) owner.StateMachine.ChangeState(PlayerState.Fall);
            }
        }

        public override void Exit(Player owner)
        {
            timer?.Dispose();
            timer = null;
            owner.GhostTimer.Stop();
            owner.DashParticles.Emitting = false;
        }
    }
}
