using Godot;

public partial class TouchScreenButton : Godot.TouchScreenButton
{
    private const float DRAG_RADIUS = 16.0f;
    private int FingerIndex = -1;
    private Vector2 RestPos;
    private Vector2 DragOffset;
    public override void _Ready()
    {
        RestPos = GlobalPosition;
    }

    public override void _Input(InputEvent @event)
    {
        Vector2 GlobalPos;
        Vector2 LocalPos;
        if (@event is InputEventScreenTouch eventScreenTouch)
        {
            if (eventScreenTouch.Pressed && FingerIndex == -1)
            {
                GlobalPos = eventScreenTouch.Position * GetCanvasTransform();
                LocalPos = GlobalPos * GetGlobalTransform();
                var rect = new Rect2(Vector2.Zero, TextureNormal.GetSize());
                if (rect.HasPoint(LocalPos))
                {
                    FingerIndex = eventScreenTouch.Index;
                    DragOffset = GlobalPos - GlobalPosition;
                }
            }
            else if (!eventScreenTouch.Pressed && FingerIndex == eventScreenTouch.Index)
            {
                FingerIndex = -1;
                GlobalPosition = RestPos;
                Input.ActionRelease("ui_right");
                Input.ActionRelease("ui_left");
                Input.ActionRelease("ui_down");
                Input.ActionRelease("ui_up");
            }
        }
        else if (@event is InputEventScreenDrag eventScreenDrag)
        {
            if (eventScreenDrag.Index == FingerIndex)
            {
                var WishPos = eventScreenDrag.Position * GetCanvasTransform() - DragOffset;
                var Movement = (WishPos - RestPos).LimitLength(DRAG_RADIUS);
                GlobalPosition = RestPos + Movement;

                Movement /= DRAG_RADIUS;
                if (Movement.X > 0)
                {
                    Input.ActionRelease("ui_left");
                    Input.ActionPress("ui_right", Movement.X);
                }
                else if (Movement.X < 0)
                {
                    Input.ActionRelease("ui_right");
                    Input.ActionPress("ui_left", -Movement.X);
                }
                if (Movement.Y > 0)
                {
                    Input.ActionRelease("ui_up");
                    Input.ActionPress("ui_down", Movement.Y);
                }
                else if (Movement.Y < 0)
                {
                    Input.ActionRelease("ui_down");
                    Input.ActionPress("ui_up", -Movement.Y);
                }
            }
        }
    }
}
