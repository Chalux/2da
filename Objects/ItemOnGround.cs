using da.Scripts;
using da.Scripts.Objects;
using Godot;
using System;

public partial class ItemOnGround : InteractableArea
{
    [Export] public int itemId = 0;
    [Export] public int itemCount = 1;
    public override void _Ready()
    {
        base._Ready();
        Utils.CheckSaveAndFree(this);
    }
    public override void Interact()
    {
        base.Interact();
        if (GameGlobal.Instance.player?.AddItem(itemId, itemCount) ?? false)
        {
            Utils.SaveToMapData(this);
            QueueFree();
        }
    }
}
