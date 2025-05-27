using da.Scripts.Objects;
using Godot;

namespace da.Objects;

public partial class DashCountObj : Control
{
    [Export] private Label TimeoutLabel;
    [Export] private TextureProgressBar TextureProgressBar;
    [Export] private Label StackLab;

    public override void _Process(double delta)
    {
        base._Process(delta);
        var p = GameGlobal.Instance.player;
        if (p?.DashCoolDownTimer == null || !IsInstanceValid(p.DashCoolDownTimer)) return;
        // GD.Print(p.DashCoolDownTimer.TimeLeft);
        TextureProgressBar.RadialFillDegrees = (float)(p.DashCoolDownTimer.GetTimeLeft() /
            p.DashCoolDownTimer.WaitTime * 360);
        StackLab.Text = p.DashCount.ToString();
        TimeoutLabel.Text = p.DashCoolDownTimer.GetTimeLeft() > 0f ? $"{p.DashCoolDownTimer.TimeLeft:0.0}" : "";
    }
}