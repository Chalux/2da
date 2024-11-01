using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class WallJumpState : BaseState
    {
        private Player _owner;
        private SceneTreeTimer timer;
        public WallJumpState()
        {
            State = PlayerState.WallJump;
        }

        public override void Enter(Player owner)
        {
            _owner = owner;
            owner.CharactorAnimPlayer.Play("jump");
            owner.Direction = owner.GetWallNormal().X > 0 ? 1 : -1;
            owner.Velocity = new(owner.WallJumpVelocity.X * owner.GetWallNormal().X, owner.WallJumpVelocity.Y);
            owner.MoveAndSlide();
            owner.CharactorAnimPlayer.AnimationFinished += ChangeToFall;
            owner.JumpTimer.Start();
            owner.JumpRequestTimer.Stop();
            owner.HasReleasedCrouchKey = !Input.IsActionPressed("ui_down");
            owner.HasReleasedJumpKey = !Input.IsActionPressed("jump");
            owner.HasWallJumped = true;
            timer = owner.GetTree().CreateTimer(0.1f);
            //Engine.TimeScale = 0.2f;
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            if (timer == null || timer.TimeLeft == 0) PlayerStaticFunc.Move(owner, delta, owner.gravity);
        }

        public override void AfterMove(double delta, Player owner)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (owner.IsOnFloor())
            {
                if (direction.Y > 0.5)
                {
                    owner.StateMachine.ChangeState(PlayerState.Crouch);
                }
                else if (direction.X == 0)
                {
                    owner.StateMachine.ChangeState(PlayerState.Landing);
                }
                else
                {
                    owner.StateMachine.ChangeState(PlayerState.Walk);
                }
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
            else
            {
                if (direction.Y > 0.5 && !owner.IsQuickDowned && owner.HasReleasedCrouchKey)
                {
                    owner.StateMachine.ChangeState(PlayerState.QuickDown);
                }
                else if (Input.IsActionPressed("jump") && owner.HasReleasedJumpKey)
                {
                    if (owner.CheckCanJump)
                    {
                        owner.StateMachine.ChangeState(PlayerState.Jump);
                    }
                }
            }
        }

        public void ChangeToFall(StringName animName)
        {
            _owner.StateMachine.ChangeState(PlayerState.Fall);
        }

        public override void Exit(Player owner)
        {
            _owner.CharactorAnimPlayer.AnimationFinished -= ChangeToFall;
            timer?.Dispose();
            timer = null;
            //Engine.TimeScale = 1.0f;
        }
    }
}
