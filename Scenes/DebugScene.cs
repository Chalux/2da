using da.Scripts;
using da.Scripts.Objects;
using Godot;
using System;
using da.Scenes;

public partial class DebugScene : BaseView
{
    [Export] Button ExitBtn;
    [Export] Button AddItemBtn;
    [Export] TextEdit ItemIdInput;
    [Export] TextEdit ItemCountInput;

    [Export] private Button PauseBtn;

    [Export] private Button ResumeBtn;

    [Export] private Label TimeScaleLab;

    [Export] private HSlider TimeScaleSlider;

    // Called when the node enters the scene tree for the first time.
    protected override void DoShow()
    {
        base.DoShow();
        ExitBtn.Pressed += QueueFree;
        AddItemBtn.Pressed += AddItemDebug;
        PauseBtn.Pressed += () => GetTree().Paused = true;
        ResumeBtn.Pressed += () => GetTree().Paused = false;
        TimeScaleLab.Text = $"当前游戏速度：{Engine.GetTimeScale():0.00}x";
        TimeScaleSlider.Value = Engine.GetTimeScale();
        TimeScaleSlider.ValueChanged += TimeScaleSliderOnValueChanged;
    }

    private void TimeScaleSliderOnValueChanged(double value)
    {
        Engine.SetTimeScale(value);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        TimeScaleLab.Text = $"当前游戏速度：{Engine.GetTimeScale():0.00}x";
    }

    private void AddItemDebug()
    {
        var id = ItemIdInput.Text.ToInt();
        var count = 0;
        try
        {
            count = ItemCountInput.Text.ToInt();
        }
        catch (FormatException)
        {
            GD.Print("Invalid count");
        }
        catch (OverflowException)
        {
            GD.Print("Count too large");
        }

        if (count == 0) return;
        if (Datas.Items.ContainsKey(id))
        {
            if (GameGlobal.Instance.player != null)
            {
                GameGlobal.Instance.player.AddItem(id, count);
            }
        }
    }
}