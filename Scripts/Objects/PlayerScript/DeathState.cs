using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class DeathState : BaseState
    {
        public DeathState()
        {
            State = PlayerState.Death;
        }
        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("death");
            owner.Velocity = Vector2.Zero;
            owner.CollisionLayer = 0;
            owner.HitBox.SetDeferred("monitoring", false);
            owner.HurtBox.SetDeferred("monitorable", false);
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            owner.Velocity = new(owner.Velocity.X, owner.Velocity.Y + owner.gravity * (float)delta);
        }
    }
}
