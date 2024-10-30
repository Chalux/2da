using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript.BeeScript
{
    internal class BeeAttackState : EnemyState
    {
        private Bee _owner;
        private Vector2? TargetPosition = null;
        private Node2D lastTarget;
        private bool isPlayingAnim = false;
        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("idle");
            _owner = owner as Bee;
            _owner.AngryCount += 1;
            _owner.AnimPlayer.AnimationFinished += OnAnimFinished;
        }

        private void OnAnimFinished(StringName animName)
        {
            _owner.AnimPlayer.Play("idle");
            isPlayingAnim = false;
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            if (isPlayingAnim)
            {
                return;
            }
            if ((TargetPosition != null && _owner.Position.DistanceTo((Vector2)TargetPosition) < 25) || (lastTarget != null && _owner.GlobalPosition.DistanceTo(lastTarget.GlobalPosition) < 30))
            {
                _owner.AnimPlayer.Play("attack");
                isPlayingAnim = true;
                _owner.Velocity = Vector2.Zero;
                TargetPosition = null;
                return;
            }
            if (_owner.AngryCount >= 5)
            {
                var direction = _owner.GlobalPosition.DirectionTo(lastTarget.GlobalPosition);
                _owner.Velocity = _owner.Velocity.MoveToward(direction * _owner.Speed, _owner.Acceleration * (float)delta);
                _owner.Direction = direction.X > 0 ? 1 : -1;
            }
            else
            {
                var oldTarget = TargetPosition;
                TargetPosition = CalcTargetPosition();
                if (oldTarget == null && TargetPosition != null)
                {
                    lastTarget = _owner.AttackArea.GetOverlappingBodies()[0];
                    _owner.CalmDownTimer.Stop();
                    _owner.AngryCount += 1;
                }
                if (TargetPosition == null)
                {
                    _owner.Velocity = _owner.Velocity.MoveToward(Vector2.Zero, _owner.Acceleration * (float)delta);
                    if (_owner.CalmDownTimer.IsStopped()) _owner.CalmDownTimer.Start();
                }
                else
                {
                    var direction = _owner.GlobalPosition.DirectionTo((Vector2)TargetPosition);
                    _owner.Velocity = _owner.Velocity.MoveToward(direction * _owner.Speed, _owner.Acceleration * (float)delta);
                    _owner.Direction = direction.X > 0 ? 1 : -1;
                }
            }
        }

        public Vector2? CalcTargetPosition()
        {
            var bodies = _owner.AttackArea.GetOverlappingBodies();
            if (bodies.Count > 0)
            {
                return bodies[0].GlobalPosition + new Vector2(0, -20);
            }

            if (TargetPosition != null && _owner.GlobalPosition.DistanceSquaredTo((Vector2)TargetPosition) < 25)
            {
                return null;
            }

            return TargetPosition;
        }

        public override void Exit(Enemy owner)
        {
            isPlayingAnim = false;
            _owner.AnimPlayer.AnimationFinished -= OnAnimFinished;
        }
    }
}
