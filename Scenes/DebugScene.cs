using da.Scripts;
using da.Scripts.Objects;
using Godot;
using System;

public partial class DebugScene : Control
{
    [Export] Button ExitBtn;
    [Export] Button AddItemBtn;
    [Export] TextEdit ItemIdInput;
    [Export] TextEdit ItemCountInput;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ExitBtn.Pressed += QueueFree;
        AddItemBtn.Pressed += AddItemDebug;
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
