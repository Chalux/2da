using da.Scripts.Objects;
using Godot;
using System;

public partial class GameOver : Control
{
    [Export] Button TitleBtn;
    [Export] Button LoadBtn;
    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        GameGlobal.Instance.UIControl.Visible = false;
        TitleBtn.Modulate = new Color(255, 255, 255, 0);
        LoadBtn.Modulate = new Color(255, 255, 255, 0);

        await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);

        Tween tween = GetTree().CreateTween();
        tween.SetParallel();
        tween.TweenProperty(TitleBtn, "modulate", new Color(255, 255, 255, 1), 1f);
        tween.TweenProperty(LoadBtn, "modulate", new Color(255, 255, 255, 1), 1f);
        tween.Finished += () => { 
            LoadBtn.GrabFocus();

            LoadBtn.Pressed += Load;
            TitleBtn.Pressed += Title;

            LoadBtn.MouseEntered += LoadBtn.GrabFocus;
            TitleBtn.MouseEntered += TitleBtn.GrabFocus;
        };
    }

    private void Title()
    {
        _ = GameGlobal.Instance.ChangeSceneAsync("res://Scenes/TitleScene.tscn");
    }

    private void Load()
    {
        GameGlobal.Instance.LoadGame();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        GetWindow().SetInputAsHandled();
    }
}
