using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class ClimbLadderState : BaseState
    {
        public ClimbLadderState()
        {
            State = PlayerState.ClimbLadder;
        }

        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("climb_ladder");
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            if (owner.LadderRay.IsColliding())
            {
                Vector2 Direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
                if (Direction.X != 0 || Direction.Y != 0)
                {
                    owner.Velocity = Direction * Player.Speed * 0.5f;
                    owner.CharactorAnimPlayer.Play("climb_ladder");
                }
                else
                {
                    owner.Velocity = Vector2.Zero;
                    owner.CharactorAnimPlayer.Pause();
                }
            }
            else
            {
                if (owner.IsOnFloor())
                {
                    owner.StateMachine.ChangeState(PlayerState.Idle);
                }
                else
                {
                    owner.StateMachine.ChangeState(PlayerState.Fall);
                }
            }
        }
    }
}
