using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    internal class DashState : BaseState
    {
        public float oldVelocityY;
        public DashState()
        {
            State = PlayerState.Dash;
        }
        public override void Enter(Player owner)
        {
            if (!owner.TryDash() && !owner.DashTimer.IsStopped())
            {
                owner.EndDash();
            }
            else
            {
                if (Input.IsActionPressed("ui_left"))
                {
                    owner.Direction = -1;
                }
                else if (Input.IsActionPressed("ui_right"))
                {
                    owner.Direction = 1;
                }
                oldVelocityY = owner.Velocity.Y;
                owner.JumpRequestTimer.Stop();
            }
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            owner.Velocity = new(owner.DashSpeed * owner.Direction, 0);
            if (Input.IsActionPressed("ui_down") && !owner.IsQuickDowned)
            {
                owner.StateMachine.ChangeState(PlayerState.QuickDown);
            }
        }
    }
}
