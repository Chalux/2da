using Godot;

public partial class GhostNode : Sprite2D
{
    public void SetProperty(Vector2 position, Vector2 scale)
    {
        Position = position;
        Scale = scale;
    }

    private void DoGhostAnimation()
    {
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(this, "self_modulate", new Color(1, 1, 1, 0), 0.75f);
        tweener.Finished += QueueFree;
    }

    public override void _Ready()
    {
        DoGhostAnimation();
    }
}
