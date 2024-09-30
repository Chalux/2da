using Godot;

public partial class MsgBox : ColorRect
{
    [Export] Timer timer;
    [Export] public Label msg;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer.Timeout += QueueFree;
    }

    public void SetMsg(string msg)
    {
        this.msg.Text = msg;
    }
}
