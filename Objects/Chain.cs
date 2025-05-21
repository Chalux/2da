using Godot;

public partial class Chain : Node2D
{
    [Export] public Sprite2D ChainSprite;
    [Export] public RigidBody2D Hook;
    public Vector2 Arrow = Vector2.Zero;
    public Vector2 Direction = Vector2.Zero;

    public bool isFlying = false;
    public bool isHooked = false;

    public override void _Process(double delta)
    {
        base._Process(delta);

        Visible = isFlying || isHooked;
        if (!Visible) return;

        Hook.GlobalPosition = Arrow;
        Arrow = Hook.GlobalPosition;
        var position = ToLocal(Arrow);
        Hook.RotationDegrees = Mathf.RadToDeg(Position.AngleToPoint(position)) + 90;
        ChainSprite.RotationDegrees = Mathf.RadToDeg(Vector2.Zero.AngleToPoint(position)) - 90;
        //ChainSprite.Position = position;
        ChainSprite.RegionRect = new Rect2(0, 0, 10, position.Length());
    }

    public void Release()
    {
        isFlying = false;
        isHooked = false;
    }

    public void HookSomething(Node2D node)
    {
        Arrow = node.GlobalPosition;
        isFlying = false;
        isHooked = true;
        var t = (Arrow - Hook.GlobalPosition).Normalized();
        Direction = new Vector2(t.X > 0 ? 1 : -1, t.Y > 0 ? 1 : -1);
    }
}
