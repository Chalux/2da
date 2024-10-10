using da.Scripts.Objects;
using Godot;
using System;
using System.Linq;

public partial class TitleScene : Control
{
    [Export] Button StartBtn;
    [Export] Button ExitBtn;
    [Export] Button LoadBtn;
    [Export] VBoxContainer list;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameGlobal.Instance.isRunning = false;
        StartBtn.GrabFocus();
        foreach (var child in list.GetChildren().Cast<Button>())
        {
            child.MouseEntered += child.GrabFocus;
        }
        GameGlobal.Instance.UIControl.Visible = false;

        StartBtn.Pressed += StartGame;

        LoadBtn.Disabled = !GameGlobal.SaveFileExists();
        LoadBtn.Pressed += LoadGame;

        ExitBtn.Pressed += () => GetTree().Quit();
    }

    private void LoadGame()
    {
        GameGlobal.Instance.LoadGame();
    }

    private void StartGame()
    {
        GameGlobal.Instance.isRunning = true;
        GameGlobal.Instance.Teleport("res://Scenes/ForestMap.tscn");
        GameGlobal.Instance.save = new();
    }
}
