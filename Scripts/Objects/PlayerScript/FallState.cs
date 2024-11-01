using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class FallState : BaseState
    {
        public FallState()
        {
            State = PlayerState.Fall;
        }

        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("fall");
            owner.CoyoteTimer.Stop();
            owner.HasReleasedCrouchKey = !Input.IsActionPressed("ui_down");
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            PlayerStaticFunc.Move(owner, delta, owner.gravity);
        }

        public override void AfterMove(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (owner.IsOnFloor())
            {
                if (direction.Y > 0.5)
                {
                    SoundManager.Ins.PlaySFX("Land");
                    owner.StateMachine.ChangeState(PlayerState.Crouch);
                }
                else if (direction.X == 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Landing);
                }
                else
                {
                    SoundManager.Ins.PlaySFX("Land");
                    owner.StateMachine.ChangeState(PlayerState.Walk);
                }
            }
            else if (Input.IsActionPressed("ui_down") && !owner.IsQuickDowned && owner.HasReleasedCrouchKey)
            {
                owner.StateMachine.ChangeState(PlayerState.QuickDown);
            }
            else if (Input.IsActionPressed("jump") && owner.HasReleasedJumpKey)
            {
                owner.TryJump();
            }
            else if (owner.IsOnWall() && owner.HandRay.IsColliding() && owner.FootRay.IsColliding())
            {
                if (owner == null || owner.GetLastSlideCollision() == null) return;
                if (owner.GetLastSlideCollision().GetCollider().GetMeta("CantSlide", false).AsBool())
                {
                    return;
                }
                if (owner.GetLastSlideCollision().GetCollider() is TileMapLayer tml)
                {
                    if (tml.TileSet.GetCustomDataLayerByName("CantSlide") == -1)
                    {
                        owner.StateMachine.ChangeState(PlayerState.WallSliding);
                        return;
                    }
                    var coords = tml.GetCoordsForBodyRid(owner.GetLastSlideCollision().GetColliderRid());
                    if (tml.GetCellTileData(coords).GetCustomData("CantSlide").AsBool())
                    {
                        return;
                    }
                }
                owner.StateMachine.ChangeState(PlayerState.WallSliding);
            }
            else if (!owner.JumpRequestTimer.IsStopped())
            {
                if (owner.CheckCanJump && owner.HasReleasedJumpKey)
                {
                    owner.TryJump();
                }
            }
            else if (direction.Y < -0.5 && owner.LadderRay.IsColliding())
            {
                owner.StateMachine.ChangeState(PlayerState.ClimbLadder);
            }
        }
    }
}
