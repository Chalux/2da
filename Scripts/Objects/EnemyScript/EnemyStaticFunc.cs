using Godot;

namespace da.Scripts.Objects.EnemyScript
{
    internal class EnemyStaticFunc
    {
        public static void Move(Enemy enemy, float speed, float acceleration, float delta, float gravity)
        {
            Godot.Vector2 velocity = enemy.Velocity;
            velocity.X = Mathf.MoveToward(velocity.X, speed * enemy.Direction, acceleration * delta);
            velocity.Y += gravity * delta;
            velocity.Y = Mathf.Clamp(velocity.Y, float.NegativeInfinity, GameGlobal.MAX_FALL_SPEED);
            enemy.Velocity = velocity;

            enemy.MoveAndSlide();
        }
    }
}
