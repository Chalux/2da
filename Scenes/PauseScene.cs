using da.Scripts.Objects;
using Godot;

public partial class PauseScene : Control
{
    [Export] Button ResumeBtn;
    [Export] Button TitleBtn;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hide();
        ResumeBtn.Pressed += OnResumeBtnPressed;
        TitleBtn.Pressed += OnTitleBtnPressed;
        VisibilityChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged()
    {
        GetTree().Paused = Visible;
    }

    private void OnTitleBtnPressed()
    {
        _ = GameGlobal.Instance.ChangeSceneAsync("res://Scenes/TitleScene.tscn");
    }

    private void OnResumeBtnPressed()
    {
        Hide();
    }

    public void ShowPause()
    {
        Show();
        ResumeBtn.GrabFocus();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            Hide();
            GetWindow().SetInputAsHandled();
        }
    }
}
