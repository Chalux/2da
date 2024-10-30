using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript
{
    internal class EnemyDeathState : EnemyState
    {
        private Enemy _owner;
        public EnemyDeathState()
        {
            State = "death";
        }

        public override void Enter(Enemy owner)
        {
            _owner = owner;
            owner.AnimPlayer.Play("die");
            owner.HitBox.SetDeferred("monitoring", false);
            owner.HurtBox.SetDeferred("monitorable", false);
            owner.Velocity = Vector2.Zero;
            owner.AnimPlayer.AnimationFinished += EnemyDead;
        }

        private void EnemyDead(StringName animName)
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
            _owner.AnimPlayer.AnimationFinished -= EnemyDead;
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            var gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
            EnemyStaticFunc.Move(owner, 5, 2.5f, (float)delta, gravity);
        }
    }
}
