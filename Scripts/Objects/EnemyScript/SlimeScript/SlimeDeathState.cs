using Godot;

namespace da.Scripts.Objects.EnemyScript.SlimeScript
{
    internal class SlimeDeathState : EnemyState
    {
        private Enemy _owner;
        public SlimeDeathState()
        {
            State = "death";
        }

        public override void Enter(Enemy owner)
        {
            _owner = owner;
            owner.AnimPlayer.Play("die");
            owner.HitBox.Monitoring = false;
            owner.Velocity = Vector2.Zero;
            owner.AnimPlayer.AnimationFinished += SlimeDead;
        }

        private void SlimeDead(StringName animName)
        {
            if (_owner != null)
            {
                Tween tween = _owner.GetTree().CreateTween();
                var c = new Color(_owner.sprite.Modulate, 0);
                tween.TweenProperty(_owner.sprite, "modulate", c, 2f);
                tween.Finished += _owner.QueueFree;
            }
        }

        public override void Exit(Enemy owner)
        {
            _owner.AnimPlayer.AnimationFinished -= SlimeDead;
        }
    }
}
