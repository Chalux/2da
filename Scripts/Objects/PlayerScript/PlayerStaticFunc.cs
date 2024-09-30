using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal static class PlayerStaticFunc
    {
        public static void Move(Player player, double delta, float gravity)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            var velocity = player.Velocity;
            if (gravity > 0)
            {
                //var gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
                velocity.Y += gravity * (float)delta;
                velocity.Y = Mathf.Clamp(velocity.Y, float.NegativeInfinity, GameGlobal.MAX_FALL_SPEED);
            }
            float acceleration = player.IsOnFloor() ? Player.FloorAcceleration : Player.AirAcceleration;
            if (!Mathf.IsZeroApprox(direction.X))
            {
                velocity.X = Mathf.MoveToward(velocity.X, direction.X * Player.Speed, acceleration * (float)delta);
                if (Input.IsActionPressed("ui_left"))
                {
                    player.Direction = -1;
                }
                else
                {
                    player.Direction = 1;
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(player.Velocity.X, 0, acceleration * (float)delta);
            }
            player.Velocity = velocity;
        }
    }
}
