using Godot;
using System;

public partial class UnstableRock : Node2D
{
    [Export] bool isBroken = false;
    [Export] AnimationPlayer animationPlayer;
    [Export] StaticBody2D rock;
    [Export] Area2D area;
    public override void _Ready()
    {
        area.BodyEntered += OnAreaBodyEntered;
    }

    private void OnAreaBodyEntered(Node2D body)
    {
        if (!isBroken)
        {
            animationPlayer.Stop();
            animationPlayer.Play("fall");
            isBroken = true;
        }
    }

    public void CheckCollision()
    {
        if (area.HasOverlappingBodies())
        {
            animationPlayer.Stop();
            animationPlayer.Play("fall");
            isBroken = true;
        }
    }
}
