using Godot;

public partial class MsgBox : ColorRect
{
    [Export] Timer timer;
    [Export] public Label msg;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer.Timeout += DoFreeTween;
    }

    private void DoFreeTween()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(Modulate, 0), 1f);
        tween.Finished += QueueFree;
    }

    public void SetMsg(string msg)
    {
        this.msg.Text = msg;
    }
}
