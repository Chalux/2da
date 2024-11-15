using da.Objects;
using da.Scripts;
using Godot;
using System;

public partial class StaticRopePoint : Area2D
{
    [Export] public Sprite2D sprite;
    [Export] public Node2D LightTarget;
    [Export] public Node2D LightTarget2;
    [Export] Timer lightTimer;
    const float lightSpeed = 30000f;
    public override void _Ready()
    {
        base._Ready();

        if (LightTarget != null || LightTarget2 != null) BodyEntered += OnBodyEntered;

        Visible = Utils.CheckGlobalData("IsRopeUnlock");
        SetDeferred("monitoring", Visible);
        SetDeferred("monitorable", Visible);
    }

    public void OnBodyEntered(Node2D body)
    {
        Node2D target = null;
        if (body.GlobalPosition.X < GlobalPosition.X)
        {
            target = LightTarget2;
        }
        else
        {
            target = LightTarget;
        }
        if (target == null) return;
        if (lightTimer.IsStopped() && body is Player p && p.chain.isHooked)
        {
            lightTimer.Start();
            PointLight2D light = new()
            {
                Texture = ResourceLoader.Load<GradientTexture2D>("res://Resources/Assets/Sprites/PointLightTexture1.tres"),
                GlobalPosition = this.GlobalPosition,
                TextureScale = 2,
                Energy = 2,
            };
            GetParent()?.AddChild(light);
            Tween tween = CreateTween();
            tween.TweenProperty(light, "global_position", target.GlobalPosition, target.GlobalPosition.DistanceSquaredTo(light.GlobalPosition) / lightSpeed);
            tween.TweenProperty(light, "texture_scale", 6, 0.5f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Elastic);
            tween.TweenProperty(light, "energy", 0, 5f);
            tween.TweenCallback(Callable.From(light.QueueFree));
        }
    }
}
