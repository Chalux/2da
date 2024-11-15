using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    public class InWaterState : BaseState
    {
        public InWaterState()
        {
            State = PlayerState.InWater;
        }

        public override void Enter(Player player)
        {
            player.CharactorAnimPlayer.Play("idle");
            player.gravity = player.gravity * 0.33f;
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            owner.CheckGrapple();
            PlayerStaticFunc.Move(owner, delta, owner.gravity);
            if (Input.IsActionPressed("jump"))
            {
                owner.Velocity = new Vector2(owner.Velocity.X, -200);
            }
            if (owner.WaterChecker.GetCollider() is Area2D a && a.GetCollisionLayerValue(10))
            {
                int tempOldValue = (int)owner.poisonCount;
                owner.poisonCount += (float)delta;
                if (tempOldValue < (int)owner.poisonCount)
                {
                    owner.status.Health -= 1;
                    SoundManager.Ins.PlaySFX("Hurt");
                    GameGlobal.Instance.ShakeCamera(2);
                }
            }
        }

        public override void AfterMove(double delta, Player owner)
        {
            if (!owner.WaterChecker.IsColliding())
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

        public override void Exit(Player owner)
        {
            owner.gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
            owner.poisonCount = 0;
        }
    }
}
