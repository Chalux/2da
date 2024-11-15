using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class WallSlidingState : BaseState
    {
        public WallSlidingState()
        {
            State = PlayerState.WallSliding;
        }

        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("wall_sliding");
            owner.Velocity = Vector2.Zero;
        }

        public override void Update(double delta, Player owner)
        {
            base.Update(delta, owner);

            owner.CheckGrapple();
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            PlayerStaticFunc.Move(owner, delta, owner.gravity / 3);
        }

        public override void AfterMove(double delta, Player owner)
        {
            if (owner.IsOnFloor())
            {
                owner.StateMachine.ChangeState(PlayerState.Idle);
            }
            else if (!owner.HandRay.IsColliding() || !owner.FootRay.IsColliding())
            {
                owner.StateMachine.ChangeState(PlayerState.Fall);
            }
            else if (!owner.JumpRequestTimer.IsStopped())
            {
                owner.StateMachine.ChangeState(PlayerState.WallJump);
            }
            else
            {
                if (owner == null || owner.GetLastSlideCollision() == null) return;
                if (owner.GetLastSlideCollision().GetCollider() is TileMapLayer tml)
                {
                    if (tml.TileSet.GetCustomDataLayerByName("CantSlide") == -1)
                    {
                        return;
                    }
                    var coords = tml.GetCoordsForBodyRid(owner.GetLastSlideCollision().GetColliderRid());
                    var customData = tml.GetCellTileData(coords).GetCustomData("CantSlide");
                    if (customData.AsBool())
                    {
                        owner.StateMachine.ChangeState(PlayerState.Fall);
                    }
                }
                else if (owner.GetLastSlideCollision().GetCollider().GetMeta("CantSlide", false).AsBool())
                {
                    owner.StateMachine.ChangeState(PlayerState.Fall);
                }
            }
        }
    }
}
