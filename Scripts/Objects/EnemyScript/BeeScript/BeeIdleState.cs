using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript.BeeScript
{
    public class BeeIdleState : EnemyState
    {
        private Bee _owner;
        private Vector2? CurrTargetPoint = null;
        public BeeIdleState()
        {
            State = "idle";
        }

        public override void OnAdd(Enemy owner)
        {
            _owner = owner as Bee;
            _owner.IdleTimer.Timeout += () => MoveToRandomPosition();
        }

        public override void Enter(Enemy owner)
        {
            owner.AnimPlayer.Play("idle");
            _owner.IdleTimer.Start();
            owner.Velocity = Vector2.Zero;
            MoveToRandomPosition();
        }

        public override void PhysicsProcess(double delta, Enemy owner)
        {
            var bodies = _owner.AttackArea.GetOverlappingBodies();
            if (bodies.Count == 0)
            {
                if (_owner.IdleTimer.IsStopped())
                {
                    MoveToRandomPosition((float)delta);
                    _owner.IdleTimer.Start();
                }
                else if (CurrTargetPoint != null)
                {
                    MoveTo((Vector2)CurrTargetPoint, (float)delta);
                }
            }
            else
            {
                _owner.StateMachine.ChangeState("attack");
            }
        }

        public override void Exit(Enemy owner)
        {
            _owner.IdleTimer.Stop();
        }

        private void MoveToRandomPosition(float delta = 0)
        {
            if (_owner.ActionShape.Shape is not CircleShape2D) return;
            var Point = _owner.ActionArea.Position;
            Random r = new();
            int randomAngle = r.Next(0, 360);
            int randomDistance = r.Next(0, (int)(_owner.ActionShape.Shape as CircleShape2D).Radius);
            Point.X += (float)Math.Cos(randomAngle) * randomDistance;
            Point.Y += (float)Math.Sin(randomAngle) * randomDistance;
            CurrTargetPoint = Point;
            MoveTo((Vector2)CurrTargetPoint, delta);
        }

        private void MoveTo(Vector2 Point, float delta)
        {
            var direction = _owner.Position.DirectionTo(Point);
            _owner.Velocity = _owner.Velocity.MoveToward(direction * _owner.Speed, delta * _owner.Acceleration);
        }
    }
}
