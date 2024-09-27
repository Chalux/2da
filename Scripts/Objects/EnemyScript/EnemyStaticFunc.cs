using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.EnemyScript
{
    internal class EnemyStaticFunc
    {
        public static void Move(Enemy enemy, float speed, float acceleration, float delta, float gravity)
        {
            Godot.Vector2 velocity = enemy.Velocity;
            velocity.X = Mathf.MoveToward(velocity.X, speed * enemy.Direction, acceleration * delta);
            velocity.Y += gravity * delta;
            enemy.Velocity = velocity;

            enemy.MoveAndSlide();
        }
    }
}
