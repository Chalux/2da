using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class DeathState : BaseState
    {
        private Player player;
        public DeathState()
        {
            State = PlayerState.Death;
        }
        public override void Enter(Player owner)
        {
            player = owner;
            owner.CharactorAnimPlayer.Play("death");
            owner.CharactorAnimPlayer.AnimationFinished += EndAnim;
            owner.Velocity = Vector2.Zero;
            owner.CollisionLayer = 0;
            owner.HitBox.SetDeferred("monitoring", false);
            owner.HurtBox.SetDeferred("monitorable", false);
        }

        private async void EndAnim(StringName animName)
        {
            await player.ToSignal(player.GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
            _ = GameGlobal.Instance.ChangeSceneAsync("res://Scenes/GameOver.tscn");
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            owner.Velocity = new(owner.Velocity.X, owner.Velocity.Y + owner.gravity * (float)delta);
        }
    }
}
