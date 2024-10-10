using da.Scripts.Objects;
using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
    private float ShakeStrength = 0f;
    [Export] private float RecoverSpeed = 16f;

    public override void _Ready()
    {
        GameGlobal.Instance.CameraShake += Shake;
    }

    public void Shake(float strength)
    {
        ShakeStrength = strength;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (ShakeStrength > 0)
        {
            Random random = new();
            Offset = new(random.NextSingle() * ShakeStrength, random.NextSingle() * ShakeStrength);
            ShakeStrength = Mathf.MoveToward(ShakeStrength, 0, RecoverSpeed * (float)delta);
        }
    }
}
